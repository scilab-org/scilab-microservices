namespace User.Api.Models;

public class UpdateUserRequest
{
    #region Fields, Properties and Indexers

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool Enabled { get; set; } = true;

    public string? GroupNames { get; set; }

    public IFormFile? AvatarImage { get; set; }

    #endregion
}
