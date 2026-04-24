using AutoMapper;
using Lab.Application.Dtos.PaperAuthors;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using Marten.Pagination;

namespace Lab.Application.Features.PaperAuthor.Queries.GetPaperAuthors;

public record GetPaperAuthorsQuery(GetPaperAuthorsFilter Filter, PaginationRequest Paging) : IQuery<GetPaperAuthorsResult>;

public class GetPaperAuthorsQueryHandler(IDocumentSession session, IMapper mapper)
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
}
