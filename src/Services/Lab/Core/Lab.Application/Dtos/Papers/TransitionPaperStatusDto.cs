using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Papers;

public class TransitionPaperStatusDto
{
    public Guid ProjectId { get; set; }
    public SubmissionStatus TargetStatus { get; set; }
    public string? Note { get; set; }
    public string? RevisionType { get; set; }
    public Guid? PdfFileId { get; set; }
    public string? SubmittedUrl { get; set; }
}
