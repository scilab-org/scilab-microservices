using Lab.Domain.Abstractions;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public sealed class TemplateEntity: Entity<Guid>
{
    #region Fields, Properties and Indexers
    
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public object TemplateStructure { get; set; } = null!;
    #endregion
    
    #region Factories
    public static TemplateEntity Create(Guid id, string name, string? code, string? description, object templateStructure)
    {
        return new TemplateEntity()
        {
            Id = id,
            Name = name,
            Code = code,
            Description = description,
            TemplateStructure = templateStructure,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }

    #endregion

    #region Methods

    public void Update(string? description, object templateStructure)
    {
        Description = description ?? Description;
        TemplateStructure = templateStructure;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    #endregion
}
