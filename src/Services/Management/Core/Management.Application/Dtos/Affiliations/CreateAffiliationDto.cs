using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Affiliations;

[ExcludeFromCodeCoverage]
public sealed class CreateAffiliationDto
{
    public required string Name { get; set; }
    public string? ShortName { get; set; }
    public required string RorId { get; set; }
    public string? RorUrl { get; set; }
}
