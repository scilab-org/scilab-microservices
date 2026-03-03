namespace Lab.Application.Dtos.Template;

public class CreateTemplateVersionDto
{
    public object TemplateStructure { get; set; } = null!;
    public string? Description { get; set; }
}