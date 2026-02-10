#region using

using System.Text.Json.Serialization;

#endregion

namespace User.Application.Models.Responses.Externals;

public sealed class KeycloakGroupResponse
{
    #region Fields, Properties and Indexers

    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("subGroups")]
    public List<KeycloakGroupResponse>? SubGroups { get; set; }

    #endregion
}
