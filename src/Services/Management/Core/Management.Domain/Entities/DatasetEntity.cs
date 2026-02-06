using Management.Domain.Abstractions;
using Management.Domain.Enums;

namespace Management.Domain.Entities;

public sealed class DatasetEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? FilePath { get; set; }
    public DatasetStatus? Status { get; set; }
    
    #endregion
    
    #region Factories
    
    public static DatasetEntity Create(Guid id,
        string name,
        string? description)
    {
        return new DatasetEntity()
        {
            Id = id,
            Name = name,
            Description = description,
            Status = DatasetStatus.Public,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }
    #endregion
    
    #region Methods
    public void Update(string name,
        string description,
        DatasetStatus? status)
    {
        Name = name;
        Description = description;
        Status = status ?? DatasetStatus.Public;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }
    
    public void UpdateFilePath(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return;

        FilePath = url;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }
    
    #endregion
    
}