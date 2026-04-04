namespace User.Application.Dtos.Users;

public sealed class CreateUserDto
{
    #region Fields, Properties and Indexers

    public string Username { get; set; } = default!;

    public string Email { get; set; } = default!;

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string InitialPassword { get; set; } = default!;

    public bool TemporaryPassword { get; set; } = true;

    public UploadFileBytes? AvatarImage { get; set; }

    public List<string>? GroupNames { get; set; }

    #endregion
}
