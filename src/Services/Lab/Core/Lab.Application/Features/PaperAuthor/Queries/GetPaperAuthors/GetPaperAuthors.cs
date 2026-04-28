using AutoMapper;
using Lab.Application.Dtos.PaperAuthors;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using Marten.Pagination;

namespace Lab.Application.Features.PaperAuthor.Queries.GetPaperAuthors;

public record GetPaperAuthorsQuery(GetPaperAuthorsFilter Filter, PaginationRequest Paging) : IQuery<GetPaperAuthorsResult>;

public class GetPaperAuthorsQueryHandler(
    IDocumentSession session,
    IMapper mapper,
    IManagementApiService managementApiService)
    : IQueryHandler<GetPaperAuthorsQuery, GetPaperAuthorsResult>
{
    public async Task<GetPaperAuthorsResult> Handle(GetPaperAuthorsQuery request, CancellationToken cancellationToken)
    {
        var query = session.Query<PaperAuthorEntity>().AsQueryable();

        if (!request.Filter.Name.IsNullOrWhiteSpace())
        {
            var name = request.Filter.Name.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(name));
        }

        if (request.Filter.PaperId.HasValue && request.Filter.PaperId.Value != Guid.Empty)
        {
            query = query.Where(x => x.PaperId == request.Filter.PaperId.Value);
        }

        if (!request.Filter.RoleName.IsNullOrWhiteSpace())
        {
            var roleName = request.Filter.RoleName.Trim().ToLower();
            var roleIds = await session.Query<AuthorRoleEntity>()
                .Where(x => x.Name.ToLower().Contains(roleName))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            query = query.Where(x => roleIds.Contains(x.AuthorRoleId));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var results = await query.OrderByDescending(x => x.CreatedOnUtc)
            .ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize, cancellationToken);

        var items = mapper.Map<List<PaperAuthorDto>>(results.ToList());
        await ApplyAuthorRoleDetailsAsync(items, cancellationToken);
        await ApplyAffiliationDetailsAsync(items, cancellationToken);

        return new GetPaperAuthorsResult(items, totalCount, request.Paging);
    }

    private async Task ApplyAuthorRoleDetailsAsync(List<PaperAuthorDto> items, CancellationToken cancellationToken)
    {
        var roleIds = items.Select(x => x.AuthorRoleId).Distinct().ToList();
        if (roleIds.Count == 0)
            return;

        var roles = await session.Query<AuthorRoleEntity>()
            .Where(x => roleIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var roleLookup = roles.ToDictionary(x => x.Id);

        foreach (var item in items)
        {
            if (roleLookup.TryGetValue(item.AuthorRoleId, out var role))
            {
                item.AuthorRoleName = role.Name;
                item.AuthorRoleDescription = role.Description;
            }
        }
    }

    private async Task ApplyAffiliationDetailsAsync(List<PaperAuthorDto> items, CancellationToken cancellationToken)
    {
        var affiliationPairs = items
            .Where(x => x.MemberId != Guid.Empty && x.AffiliationId != Guid.Empty)
            .Select(x => new { x.MemberId, x.AffiliationId })
            .DistinctBy(x => (x.MemberId, x.AffiliationId))
            .ToList();

        if (affiliationPairs.Count == 0)
            return;

        var memberTasks = affiliationPairs
            .Select(async pair =>
            {
                var member = await managementApiService.GetMemberByIdAsync(pair.MemberId, cancellationToken);
                if (member is null)
                    return ((Guid MemberId, Guid AffiliationId, ManagementUserAffiliationInfo? Info)?)null;

                var affiliation = await managementApiService.GetUserAffiliationByUserIdAndAffiliationIdAsync(
                    member.UserId,
                    pair.AffiliationId,
                    cancellationToken);

                return (pair.MemberId, pair.AffiliationId, Info: affiliation);
            })
            .ToList();

        var affiliations = await Task.WhenAll(memberTasks);
        var lookup = affiliations
            .Where(x => x.HasValue && x.Value.Info is not null)
            .Select(x => x!.Value)
            .ToDictionary(x => (x.MemberId, x.AffiliationId), x => x.Info!);

        foreach (var item in items)
        {
            if (lookup.TryGetValue((item.MemberId, item.AffiliationId), out var affiliation))
            {
                item.Department = affiliation.Department;
                item.Position = affiliation.Position;
                item.AffiliationStartYear = affiliation.AffiliationStartYear;
                item.AffiliationEndYear = affiliation.AffiliationEndYear;
            }
        }
    }
}
