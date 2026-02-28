namespace Management.Application.Dtos.Members;

public sealed class UserInfoDto
{
    #region Fields, Properties and Indexers

    public string Id { get; set; } = default!;

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool Enabled { get; set; }

    /// <summary>
    /// Keycloak group names the user belongs to (e.g. "admin", "manager", "user").
    /// Used to determine the user's role in the project.
    /// </summary>
    public List<string> Groups { get; set; } = new();

    #endregion
}

