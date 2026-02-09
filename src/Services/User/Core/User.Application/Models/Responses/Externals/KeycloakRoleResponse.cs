#region using

using System.Text.Json.Serialization;

#endregion

namespace User.Application.Models.Responses.Externals;

public sealed class KeycloakRoleResponse
{
    #region Fields, Properties and Indexers

    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("composite")]
    public bool Composite { get; set; }

    [JsonPropertyName("clientRole")]
    public bool ClientRole { get; set; }

    #endregion
}
