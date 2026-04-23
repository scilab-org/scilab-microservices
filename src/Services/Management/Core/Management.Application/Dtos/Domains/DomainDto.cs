using Management.Application.Dtos.Abstractions;

using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Domains;

[ExcludeFromCodeCoverage]
public sealed class DomainDto : DtoId<Guid>, IAuditableDto
{
    #region Fields, Properties and Indexers

    public string? Name { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModifiedOnUtc { get; set; }
    public string? LastModifiedBy { get; set; }

    #endregion
}
