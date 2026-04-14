using Lab.Domain.Models;

namespace Lab.Application.Dtos.Template;

public class TemplateDto
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public Guid ConferenceJournalId { get; set; }
    public List<Section>? Sections { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? LastModifiedOnUtc { get; set; }
}