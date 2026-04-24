using Lab.Application.Dtos.AuthorRoles;

namespace Lab.Application.Models.Results;

public class GetAuthorRoleByIdResult
{
    public AuthorRoleDto AuthorRole { get; init; }

    public GetAuthorRoleByIdResult(AuthorRoleDto authorRole)
    {
        AuthorRole = authorRole;
    }
}
