using Microsoft.AspNetCore.Http;

namespace Management.Application.Dtos.Datasets;

public class CreateDatasetDto
{
    #region Fields, Properties and Indexers
    
    public required Guid ProjectId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required UploadFileBytes? UploadFile { get; set; }
    
    #endregion
}