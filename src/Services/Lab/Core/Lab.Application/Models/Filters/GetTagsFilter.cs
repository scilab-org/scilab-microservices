namespace Lab.Application.Models.Filters;

public record class GetTagsFilter
{
    public string? Name { get; set; }
    public bool? IsDeleted { get; set; } = false;
}