namespace User.Api.Models;

public class CreateUserRequest
{
    #region Fields, Properties and Indexers

    public required string Username { get; set; }

    public required string Email { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }
    
    public string? OcrId { get; set; }

    public string InitialPassword { get; set; } = default!;

    public bool TemporaryPassword { get; set; } = true;

    public string? GroupNames { get; set; }

    public IFormFile? AvatarImage { get; set; }

    #endregion
}
