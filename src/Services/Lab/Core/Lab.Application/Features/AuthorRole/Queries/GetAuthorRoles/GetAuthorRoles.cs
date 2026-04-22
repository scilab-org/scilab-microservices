using AutoMapper;
using Lab.Application.Dtos.AuthorRoles;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using Marten.Pagination;

namespace Lab.Application.Features.AuthorRole.Queries.GetAuthorRoles;

public record GetAuthorRolesQuery(string? Name, PaginationRequest Paging) : IQuery<GetAuthorRolesResult>;

public class GetAuthorRolesQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetAuthorRolesQuery, GetAuthorRolesResult>
{
    public async Task<GetAuthorRolesResult> Handle(GetAuthorRolesQuery request, CancellationToken cancellationToken)
    {
        var paging = request.Paging;
        var query = session.Query<AuthorRoleEntity>().AsQueryable();

        if (!request.Name.IsNullOrWhiteSpace())
        {
            var name = request.Name.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(name));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var results = await query.OrderByDescending(x => x.CreatedOnUtc)
            .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        return new GetAuthorRolesResult(mapper.Map<List<AuthorRoleDto>>(results.ToList()), totalCount, paging);
    }
}
