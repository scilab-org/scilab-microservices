#region using

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

#endregion

namespace User.Application.Models.Requests.Externals;

[ExcludeFromCodeCoverage]
public sealed class KeycloakUpdateUserRequest
{
    #region Fields, Properties and Indexers

    [JsonPropertyName("firstName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LastName { get; set; }

    [JsonPropertyName("enabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Enabled { get; set; }

    [JsonPropertyName("attributes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, List<string>>? Attributes { get; set; }

    #endregion
}
