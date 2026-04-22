namespace User.Api.Models;

public class CreateUserRequest
{
    #region Fields, Properties and Indexers

    public string Username { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string InitialPassword { get; set; } = default!;

    public bool TemporaryPassword { get; set; } = true;

    public string? GroupNames { get; set; }

    public string? OcrId { get; set; }

    public IFormFile? AvatarImage { get; set; }

    #endregion
}
