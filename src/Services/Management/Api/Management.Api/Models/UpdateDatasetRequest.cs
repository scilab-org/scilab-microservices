namespace Management.Api.Models;

public class UpdateDatasetRequest
{
    #region Fields, Properties and Indexers
    
    public required string Name { get; set; }
    public string? Description { get; set; } 
    public IFormFile? File { get; set; }
    
    #endregion
}