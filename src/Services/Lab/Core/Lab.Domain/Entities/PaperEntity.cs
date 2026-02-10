using Lab.Domain.Abstractions;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public sealed class PaperEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string Title { get; set; } = null!;
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public string? FilePath { get; set; }
    public PaperStatus? Status { get; set; }
    public DateTimeOffset? PublicationDate { get; set; }
    public string? PaperType { get; set; }
    public string? JournalName { get; set; }
    public string? ConferenceName { get; set; }

    #endregion

    #region Factories

    public static PaperEntity Create(Guid id,
        string title,
        string? abstractText,
        string? doi,
        PaperStatus? status,
        DateTimeOffset? publicationDate,
        string? paperType,
        string? journalName,
        string? conferenceName)
    {
        return new PaperEntity()
        {
            Id = id,
            Title = title,
            Abstract = abstractText,
            Doi = doi,
            Status = status ?? PaperStatus.Draft,
            PublicationDate = publicationDate,
            PaperType = paperType,
            JournalName = journalName,
            ConferenceName = conferenceName,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }

    #endregion

    #region Methods

    public void Update(string? title,
        string? abstractText,
        string? doi,
        PaperStatus? status,
        DateTimeOffset? publicationDate,
        string? paperType,
        string? journalName,
        string? conferenceName)
    {
        Title = title ?? Title;
        Abstract = abstractText;
        Doi = doi;
        Status = status;
        PublicationDate = publicationDate;
        PaperType = paperType;
        JournalName = journalName;
        ConferenceName = conferenceName;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    public void UpdateFilePath(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return;

        FilePath = url;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    #endregion
}