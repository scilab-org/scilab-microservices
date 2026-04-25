using Management.Application.Dtos.Affiliations;
using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.UserAffiliations;

[ExcludeFromCodeCoverage]
public sealed class UserAffiliationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public AffiliationDto? Affiliation { get; set; }
    public string? Department { get; set; }
    public string? Position { get; set; }
    public int? AffiliationStartYear { get; set; }
    public int? AffiliationEndYear { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset LastModifiedOnUtc { get; set; }
}
