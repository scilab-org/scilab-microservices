using Management.Application.Dtos.Abstractions;

using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Domains;

[ExcludeFromCodeCoverage]
public sealed class DomainDto : DtoId<Guid>
{
    #region Fields, Properties and Indexers

    public string? Name { get; set; }
    public string? Description { get; set; }

    #endregion
}
