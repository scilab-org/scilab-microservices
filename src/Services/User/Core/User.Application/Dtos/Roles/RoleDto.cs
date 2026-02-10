namespace User.Application.Dtos.Roles;

public sealed class RoleDto
{
    #region Fields, Properties and Indexers

    public string Id { get; set; } = default!;

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool Composite { get; set; }

    public bool ClientRole { get; set; }

    #endregion
}
