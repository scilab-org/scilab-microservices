#region using

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

#endregion

namespace User.Application.Models.Requests.Externals;

[ExcludeFromCodeCoverage]
public sealed class KeycloakResetPasswordRequest
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
