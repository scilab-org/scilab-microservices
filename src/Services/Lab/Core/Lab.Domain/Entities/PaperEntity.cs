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
    public string? ParsedText { get; set; }
    public bool? IsIngested { get; set; } = false;
    public bool? IsAutoTagged { get; set; } = false;
    public DateTimeOffset? PublicationDate { get; set; }
    public string? PaperType { get; set; }
    public string? JournalName { get; set; }
    public string? ConferenceName { get; set; }
    public List<string> TagNames { get; set; } = new();

    #endregion

    #region Factories

    public static PaperEntity Create(Guid id,
        string title,
        string? abstractText,
        string? doi,
        PaperStatus? status,
        string? parsedText,
        bool? isIngested,
        bool? isAutoTagged,
        DateTimeOffset? publicationDate,
        string? paperType,
        string? journalName,
        string? conferenceName,
        List<string>? tagNames = null)
    {
        return new PaperEntity()
        {
            Id = id,
            Title = title,
            Abstract = abstractText,
            Doi = doi,
            Status = status ?? PaperStatus.Draft,
            ParsedText = parsedText ?? string.Empty,
            IsIngested = isIngested ?? false,
            IsAutoTagged = isAutoTagged ?? false,
            PublicationDate = publicationDate,
            PaperType = paperType,
            JournalName = journalName,
            ConferenceName = conferenceName,
            TagNames = tagNames ?? new(),
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
        bool? isIngested,
        bool? isAutoTagged,
        DateTimeOffset? publicationDate,
        string? paperType,
        string? journalName,
        string? conferenceName,
        List<string>? tagNames = null)
    {
        Title = title ?? Title;
        Abstract = abstractText;
        Doi = doi;
        Status = status;
        IsIngested = isIngested ?? IsIngested;
        IsAutoTagged = isAutoTagged ?? IsAutoTagged;
        PublicationDate = publicationDate;
        PaperType = paperType;
        JournalName = journalName;
        ConferenceName = conferenceName;
        TagNames = tagNames ?? TagNames;
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