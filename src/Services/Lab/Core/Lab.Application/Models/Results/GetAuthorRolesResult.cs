using Lab.Application.Dtos.AuthorRoles;

namespace Lab.Application.Models.Results;

public sealed class GetAuthorRolesResult
{
    public List<AuthorRoleDto> Items { get; init; }
    public PagingResult Paging { get; init; }

    public GetAuthorRolesResult(List<AuthorRoleDto> items, long totalCount, PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }
}
