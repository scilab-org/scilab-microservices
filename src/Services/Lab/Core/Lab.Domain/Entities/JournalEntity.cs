using Lab.Domain.Abstractions;

namespace Lab.Domain.Entities;

public class ConferenceJournalEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string Name { get; set; } = null!;
    public Guid ProjectId { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public string? TexFile { get; set; }
    public string? PdfFile { get; set; }
    public string? Style { get; set; }

    #endregion

    #region Factories

    public static ConferenceJournalEntity Create(
        Guid id,
        string name,
        Guid projectId,
        DateTimeOffset startAt,
        DateTimeOffset endAt,
        string? style,
        string? texFile,
        string? pdfFile,
        string? createdBy = null)
    {
        return new ConferenceJournalEntity()
        {
            Id = id,
            Name = name,
            ProjectId = projectId,
            StartAt = startAt,
            EndAt = endAt,
            Style = style,
            TexFile = texFile,
            PdfFile = pdfFile,
            CreatedBy = createdBy,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }

    #endregion

    #region Methods

    public void Update(
        string? name = null,
        Guid? projectId = null,
        DateTimeOffset? startAt = null,
        DateTimeOffset? endAt = null,
        string? style = null,
        string? texFile = null,
        string? pdfFile = null,
        string? lastModifiedBy = null)
    {
        Name = name ?? Name;
        ProjectId = projectId ?? ProjectId;
        StartAt = startAt ?? StartAt;
        EndAt = endAt ?? EndAt;
        Style = style ?? Style;
        TexFile = texFile ?? TexFile;
        PdfFile = pdfFile ?? PdfFile;
        LastModifiedBy = lastModifiedBy ?? LastModifiedBy;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    public void UpdateFilePath(string? texFileUrl, string? pdfFileUrl)
    {
        if (string.IsNullOrWhiteSpace(texFileUrl) && string.IsNullOrWhiteSpace(pdfFileUrl)) return;
        TexFile = texFileUrl;
        PdfFile = pdfFileUrl;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    #endregion
}