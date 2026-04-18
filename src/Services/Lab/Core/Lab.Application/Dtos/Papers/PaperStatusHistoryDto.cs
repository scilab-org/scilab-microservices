using Lab.Application.Dtos.Abstractions;
using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Papers;

public class PaperStatusHistoryDto : ICreationAuditDto
{
    public Guid Id { get; set; }
    public Guid PaperId { get; set; }
    public SubmissionStatus Status { get; set; }
    public Guid ActorId { get; set; }
    public string ActorUserName { get; set; } = null!;
    public string? Note { get; set; }
    public string? RevisionType { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string? CreatedBy { get; set; }
}
