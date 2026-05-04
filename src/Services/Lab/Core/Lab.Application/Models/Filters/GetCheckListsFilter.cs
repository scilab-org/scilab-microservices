namespace Lab.Application.Models.Filters;

public record class GetCheckListsFilter
{
    public string? Section { get; set; }
    public string? RuleName { get; set; }
    public string? Item { get; set; }
    public int? Weight { get; set; }
    public bool? IsDeleted { get; set; } = false;
}
