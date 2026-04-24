#region using

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

#endregion

namespace User.Application.Models.Responses.Externals;

[ExcludeFromCodeCoverage]
public sealed class KeycloakUserResponse
{
    #region Fields, Properties and Indexers

    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }
    
    [JsonPropertyName("ocrId")]
    public string? OcrId { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; set; }

    [JsonPropertyName("createdTimestamp")]
    public long CreatedTimestamp { get; set; }

    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>>? Attributes { get; set; }

    #endregion
}
