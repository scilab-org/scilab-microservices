using Management.Application.Dtos.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Affiliations;

[ExcludeFromCodeCoverage]
public sealed class AffiliationDto : DtoId<Guid>, IAuditableDto
{
    public string? Name { get; set; }
    public string? ShortName { get; set; }
    public string? RorId { get; set; }
    public string? RorUrl { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModifiedOnUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}
