using Management.Domain.Abstractions;

namespace Management.Application.Dtos.Abstractions;

public abstract class AuditableDto : IAuditable
{
    #region Fields, Properties and Indexers

    public DateTimeOffset CreatedOnUtc { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset? LastModifiedOnUtc { get; set; }

    public string? LastModifiedBy { get; set; }

    #endregion
}