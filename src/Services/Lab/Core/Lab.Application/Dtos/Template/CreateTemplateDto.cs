using Lab.Domain.Models;

namespace Lab.Application.Dtos.Template;

public class CreateTemplateDto
{
    public string? Code { get; set; }
    public string? Description { get; set; }
    public List<Section>? Sections { get; set; }
}