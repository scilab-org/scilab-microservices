using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Domains;

[ExcludeFromCodeCoverage]
public sealed class CreateDomainDto
{
    #region Fields, Properties and Indexers

    public required string Name { get; set; }
    public string? Description { get; set; }

    #endregion
}
