using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.UserAffiliations;

[ExcludeFromCodeCoverage]
public sealed class UpdateUserAffiliationDto
{
    public Guid? UserId { get; set; }
    public Guid? AffiliationId { get; set; }
    public string? Department { get; set; }
    public string? Position { get; set; }
    public int? AffiliationStartYear { get; set; }
    public int? AffiliationEndYear { get; set; }
}
