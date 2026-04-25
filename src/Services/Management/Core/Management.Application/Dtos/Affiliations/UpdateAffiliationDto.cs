using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Affiliations;

[ExcludeFromCodeCoverage]
public sealed class UpdateAffiliationDto
{
    public string? Name { get; set; }
    public string? ShortName { get; set; }
    public string? RorId { get; set; }
    public string? RorUrl { get; set; }
}
