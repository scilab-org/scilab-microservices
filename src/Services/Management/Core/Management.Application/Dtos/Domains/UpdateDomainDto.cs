using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Domains;

[ExcludeFromCodeCoverage]
public sealed class UpdateDomainDto
{
    #region Fields, Properties and Indexers

    public required string Name { get; set; }

    #endregion
}
