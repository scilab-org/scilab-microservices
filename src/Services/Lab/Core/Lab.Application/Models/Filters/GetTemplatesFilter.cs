
namespace Lab.Application.Models.Filters;

public record GetTemplatesFilter
{
    public string? Code { get; set; }
    public string? Description { get; set; }
}