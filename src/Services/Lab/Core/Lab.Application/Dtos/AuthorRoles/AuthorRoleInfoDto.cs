using Lab.Application.Dtos.Abstractions;

namespace Lab.Application.Dtos.AuthorRoles;

public class AuthorRoleInfoDto : DtoId<Guid>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
