namespace User.Application.Dtos.Users;

public sealed class CreateUserDto
{
    #region Fields, Properties and Indexers

    public string Username { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string InitialPassword { get; set; } = default!;

    public bool TemporaryPassword { get; set; } = true;

    public List<string>? GroupNames { get; set; }

    #endregion
}
