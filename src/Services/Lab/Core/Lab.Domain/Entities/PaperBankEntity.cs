using Lab.Domain.Abstractions;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public sealed class PaperBankEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string Title { get; set; } = null!;
    public string? Authors { get; set; }
    public string? Publisher { get; set; }
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public string? FilePath { get; set; }
    public string? ParsedText { get; set; }
    public bool? IsIngested { get; set; } = false;
    public bool? IsAutoTagged { get; set; } = false;
    public DateTimeOffset? PublicationDate { get; set; }
    public string? PaperType { get; set; }
    public string? JournalName { get; set; }
    public string? Pages {get; set;}
    public string? Number { get; set; }
    public string? Volume {get; set;}
    public string? ConferenceName { get; set; }
    public string? ReferenceContent { get; set; }
    public List<string> TagNames { get; set; } = new();
    public IngestStatus? IngestStatus { get; set; }

    #endregion

    #region Factories

    public static PaperBankEntity Create(Guid id,
        string title,
        string? authors = null,
        string? publisher = null,
        string? abstractText = null,
        string? doi = null,
        string? parsedText = null,
        bool? isIngested = null,
        bool? isAutoTagged = null,
        DateTimeOffset? publicationDate = null,
        string? paperType = null,
        string? journalName = null,
        string? pages = null,
        string? number = null,
        string? volume = null,
        string? conferenceName = null,
        string? referenceContent = null,
        List<string>? tagNames = null,
        IngestStatus? ingestStatus = null)
    {
        return new PaperBankEntity()
        {
            Id = id,
            Title = title,
            Authors = authors,
            Publisher = publisher,
            Abstract = abstractText,
            Doi = doi,
            ParsedText = parsedText ?? string.Empty,
            IsIngested = isIngested ?? false,
            IsAutoTagged = isAutoTagged ?? false,
            PublicationDate = publicationDate,
            PaperType = paperType,
            JournalName = journalName,
            Pages = pages,
            Number = number,
            Volume = volume,
            ConferenceName = conferenceName,
            ReferenceContent = referenceContent,
            TagNames = tagNames ?? new(),
            IngestStatus = ingestStatus ?? Enums.IngestStatus.Pending,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }

    #endregion

    #region Methods

    public void Update(string? title = null,
        string? authors = null,
        string? publisher = null,
        string? abstractText = null,
        string? doi = null,
        bool? isIngested = null,
        bool? isAutoTagged = null,
        DateTimeOffset? publicationDate = null,
        string? paperType = null,
        string? journalName = null,
        string? pages = null,
        string? number = null,
        string? volume = null,
        string? conferenceName = null,
        string? referenceContent = null,
        IngestStatus? ingestStatus = null,
        List<string>? tagNames = null)
    {
        Title = title ?? Title;
        Authors = authors ?? Authors;
        Publisher = publisher ?? Publisher;
        Abstract = abstractText ?? Abstract;
        Doi = doi ?? Doi;
        IsIngested = isIngested ?? IsIngested;
        IsAutoTagged = isAutoTagged ?? IsAutoTagged;
        PublicationDate = publicationDate ?? PublicationDate;
        PaperType = paperType ?? PaperType;
        JournalName = journalName ?? JournalName;
        Pages = pages ?? Pages;
        Number = number ?? Number;
        Volume = volume ?? Volume;
        ConferenceName = conferenceName ?? ConferenceName;
        ReferenceContent = referenceContent ?? ReferenceContent;
        IngestStatus = ingestStatus ?? IngestStatus;
        TagNames = tagNames ?? TagNames;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    public void UpdateIngestionStatus(bool isIngested, IngestStatus ingestStatus)
    {
        IsIngested = isIngested;
        IngestStatus = ingestStatus;
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