#region using

using System.Text.Json.Serialization;

#endregion

namespace User.Application.Models.Requests.Externals;

public sealed class KeycloakCreateUserRequest
{
    #region Fields, Properties and Indexers

    [JsonPropertyName("username")]
    public string Username { get; set; } = default!;

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;

    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; set; }

    [JsonPropertyName("credentials")]
    public List<KeycloakCredential>? Credentials { get; set; }

    [JsonPropertyName("groups")]
    public List<string>? Groups { get; set; }

    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>>? Attributes { get; set; }

    #endregion
}

public sealed class KeycloakCredential
{
    #region Fields, Properties and Indexers

    [JsonPropertyName("type")]
    public string Type { get; set; } = "password";

    [JsonPropertyName("value")]
    public string Value { get; set; } = default!;

    [JsonPropertyName("temporary")]
    public bool Temporary { get; set; } = true;

    #endregion
}
