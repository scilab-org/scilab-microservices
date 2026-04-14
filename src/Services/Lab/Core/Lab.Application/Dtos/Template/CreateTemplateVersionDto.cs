using Lab.Domain.Models;

namespace Lab.Application.Dtos.Template;

public class CreateTemplateVersionDto
{
    public string? Description { get; set; }
    public List<Section>? Sections { get; set; }
}