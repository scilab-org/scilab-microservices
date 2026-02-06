namespace Management.Api.Models;

public class CreateDatasetRequest
{
    #region Fields, Properties and Indexers
    
    public string? ProjectId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; } 
    public IFormFile? File { get; set; }
    
    #endregion
}