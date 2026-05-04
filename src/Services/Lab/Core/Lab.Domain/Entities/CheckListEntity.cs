using Lab.Domain.Abstractions;
using Lab.Domain.Models;

namespace Lab.Domain.Entities;

public class CheckListEntity : Entity<Guid>
{
    public string Section { get; set; } = null!;
    public List<Item> Items { get; set; } = [];

    #region Factories

    public static CheckListEntity Create(Guid id, string section, List<Item> items,
        string? createBy = null)
    {
        return new CheckListEntity()
        {
            Id = id,
            Section = section,
            Items = items,
            CreatedBy = createBy,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedBy = createBy,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }

    #endregion

    #region Methods

    public void Update(string? section = null, List<Item>? items = null,
        string? modifiedBy = null)
    {
        Section = section ?? Section;
        Items = items ?? Items;
        LastModifiedBy = modifiedBy;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    #endregion
}