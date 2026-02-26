namespace User.Application.Dtos.Users;

public sealed class UpdateUserDto
{
    #region Fields, Properties and Indexers

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool Enabled { get; set; } = true;

    public List<string>? GroupNames { get; set; }

    #endregion
}
