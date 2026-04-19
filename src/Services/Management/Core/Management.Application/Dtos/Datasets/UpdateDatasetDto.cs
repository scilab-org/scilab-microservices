using Management.Domain.Enums;

using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Datasets;

[ExcludeFromCodeCoverage]
public class UpdateDatasetDto
{
    #region Fields, Properties and Indexers
    
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DatasetStatus? Status { get; set; }
    public required UploadFileBytes? UploadFile { get; set; }
    
    #endregion
}