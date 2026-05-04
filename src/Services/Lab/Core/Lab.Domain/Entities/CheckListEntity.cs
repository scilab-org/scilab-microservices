using Lab.Domain.Abstractions;

namespace Lab.Domain.Entities;

public class CheckListEntity : Entity<Guid>
{
    public string Section { get; set; } = null!;
    public string RuleName { get; set; } = null!;
    public string Item { get; set; } = null!;
    public int Weight { get; set; }

    #region Factories

    public static CheckListEntity Create(Guid id, string section, string ruleName, string item, int weight,
        string? createBy = null)
    {
        return new CheckListEntity()
        {
            Id = id,
            Section = section,
            RuleName = ruleName,
            Item = item,
            Weight = weight,
            CreatedBy = createBy,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedBy = createBy,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }

    #endregion

    #region Methods

    public void Update(string? section = null, string? ruleName = null, string? item = null, int? weight = null,
        string? modifiedBy = null)
    {
        Section = section ?? Section;
        RuleName = ruleName ?? RuleName;
        Item = item ?? Item;
        Weight = weight ?? Weight;
        LastModifiedBy = modifiedBy;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    #endregion
}