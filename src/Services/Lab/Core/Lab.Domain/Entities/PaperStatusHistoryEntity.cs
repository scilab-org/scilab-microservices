using Lab.Domain.Abstractions;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public sealed class PaperStatusHistoryEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public Guid PaperId { get; set; }
    public SubmissionStatus Status { get; set; }
    public Guid ActorId { get; set; }
    public string ActorUserName { get; set; } = null!;
    public string? Note { get; set; }
    public string? RevisionType { get; set; }
    public Guid? PdfFileId { get; set; }
    public string? SubmittedUrl { get; set; }

    #endregion

    #region Factories

    public static PaperStatusHistoryEntity Create(
        Guid paperId,
        SubmissionStatus status,
        Guid actorId,
        string actorUserName,
        string? note = null,
        string? revisionType = null,
        Guid? pdfFileId = null,
        string? submittedUrl = null)
    {
        return new PaperStatusHistoryEntity
        {
            Id = Guid.NewGuid(),
            PaperId = paperId,
            Status = status,
            ActorId = actorId,
            ActorUserName = actorUserName,
            Note = note,
            RevisionType = revisionType,
            PdfFileId = pdfFileId,
            SubmittedUrl = submittedUrl,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
            CreatedBy = actorUserName
        };
    }

    #endregion
}
