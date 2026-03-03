using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Template;

public class CreateTemplateDto
{
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public object TemplateStructure { get; set; } = null!;
}