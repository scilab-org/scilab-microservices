using Lab.Domain.Enums;

namespace Lab.Application.Models.Filters;

public record class GetTemplatesFilter
{
    public string? Name { get; set; }
    public string? Code { get; set; }
}

