using Management.Application.Dtos.Abstractions;
using Management.Domain.Enums;

using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Datasets;

[ExcludeFromCodeCoverage]
public class DatasetDto : DtoId<Guid>
{
    #region Fields, Properties and Indexers
    
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? FilePath { get; set; }
    public DatasetStatus? Status { get; set; }
    #endregion
}