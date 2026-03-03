using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Template;

public class TemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public object TemplateStructure { get; set; } = null!;
    public int Version { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? LastModifiedOnUtc { get; set; }
}

