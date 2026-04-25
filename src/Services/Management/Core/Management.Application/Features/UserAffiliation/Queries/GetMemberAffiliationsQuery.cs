using AutoMapper;
using Management.Application.Dtos.Affiliations;
using Management.Application.Dtos.UserAffiliations;
using Management.Application.Models.Results;
using Management.Domain.Entities;
using Marten;
using Marten.Linq;
using MediatR;

namespace Management.Application.Features.UserAffiliation.Queries;

public sealed record GetMemberAffiliationsQuery(Guid MemberId, PaginationRequest Paging, string? AffiliationName) : IRequest<GetMemberAffiliationsResult>;

public sealed class GetMemberAffiliationsQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetMemberAffiliationsQuery, GetMemberAffiliationsResult>
{
    public async Task<GetMemberAffiliationsResult> Handle(GetMemberAffiliationsQuery query, CancellationToken cancellationToken)
    {
        var member = await session.LoadAsync<MemberEntity>(query.MemberId, cancellationToken)
            ?? throw new ClientValidationException(MessageCode.MemberNotFound, query.MemberId.ToString());

        var affiliationsQuery = session.Query<UserAffiliationEntity>()
            .Where(x => x.UserId == member.UserId)
            .OrderByDescending(x => x.CreatedOnUtc);

        var affiliations = await affiliationsQuery.ToListAsync(cancellationToken);

        var affiliationDtos = new List<UserAffiliationDto>(affiliations.Count);
        foreach (var item in affiliations)
        {
            var affiliation = await session.LoadAsync<AffiliationEntity>(item.AffiliationId, cancellationToken);
            if (!string.IsNullOrWhiteSpace(query.AffiliationName) &&
                (affiliation is null || affiliation.Name is null || !affiliation.Name.Contains(query.AffiliationName.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var dto = mapper.Map<UserAffiliationDto>(item);
            dto.Affiliation = affiliation is null ? null : mapper.Map<AffiliationDto>(affiliation);
            affiliationDtos.Add(dto);
        }

        var totalCount = affiliationDtos.Count;
        var items = affiliationDtos
            .Skip((query.Paging.PageNumber - 1) * query.Paging.PageSize)
            .Take(query.Paging.PageSize)
            .ToList();

        return new GetMemberAffiliationsResult(items, totalCount, query.Paging);
    }
}
