namespace Lab.Application.Models.Filters;

public record class GetJournalsFilter
{
    public string? Name { get; set; }
    public bool? IsDeleted { get; set; } = false;
}