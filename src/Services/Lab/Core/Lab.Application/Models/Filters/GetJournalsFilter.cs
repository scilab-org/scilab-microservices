namespace Lab.Application.Models.Filters;

public record class GetJournalsFilter
{
    public string? Name { get; set; }
    public string? TemplateCode { get; set; }
    public string? ProjectId { get; set; }
    public string? PaperId { get; set; }
    public bool? IsDeleted { get; set; } = false;
}