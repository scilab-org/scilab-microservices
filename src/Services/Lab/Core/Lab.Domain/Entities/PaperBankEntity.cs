using Lab.Domain.Abstractions;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public sealed class PaperBankEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string Title { get; set; } = null!;
    public string? Authors { get; set; }
    public string? Publisher { get; set; }
    public string? Ranking { get; set; }
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public string? FilePath { get; set; }
    public string? BibFilePath { get; set; }
    public string? Url { get; set; }
    public string? ParsedText { get; set; }
    public bool? IsIngested { get; set; } = false;
    public bool? IsAutoTagged { get; set; } = false;
    public DateTimeOffset? PublicationDate { get; set; }
    public string? PaperType { get; set; }
    public string? Pages { get; set; }
    public string? Number { get; set; }
    public string? Volume { get; set; }
    public Guid? ConferenceJournalId { get; set; }
    public string? ReferenceContent { get; set; }
    public List<string> Keywords { get; set; } = new();
    public IngestStatus? IngestStatus { get; set; }

    #endregion

    #region Factories

    public static PaperBankEntity Create(Guid id,
        string title,
        string? authors = null,
        string? publisher = null,
        string? ranking = null,
        string? abstractText = null,
        string? doi = null,
        string? url = null,
        string? parsedText = null,
        bool? isIngested = null,
        bool? isAutoTagged = null,
        DateTimeOffset? publicationDate = null,
        string? paperType = null,
        string? pages = null,
        string? number = null,
        string? volume = null,
        Guid? conferenceJournalId = null,
        string? referenceContent = null,
        List<string>? keywords = null,
        IngestStatus? ingestStatus = null)
    {
        return new PaperBankEntity()
        {
            Id = id,
            Title = title,
            Authors = authors,
            Publisher = publisher,
            Ranking = ranking,
            Abstract = abstractText,
            Doi = doi,
            Url = url,
            ParsedText = parsedText ?? string.Empty,
            IsIngested = isIngested ?? false,
            IsAutoTagged = isAutoTagged ?? false,
            PublicationDate = publicationDate,
            PaperType = paperType,
            Pages = pages,
            Number = number,
            Volume = volume,
            ConferenceJournalId = conferenceJournalId,
            ReferenceContent = referenceContent,
            Keywords = keywords ?? new(),
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
        string? ranking = null,
        string? abstractText = null,
        string? doi = null,
        string? url = null,
        bool? isIngested = null,
        bool? isAutoTagged = null,
        DateTimeOffset? publicationDate = null,
        string? paperType = null,
        string? pages = null,
        string? number = null,
        string? volume = null,
        Guid? conferenceJournalId = null,
        string? referenceContent = null,
        IngestStatus? ingestStatus = null,
        List<string>? keywords = null)
    {
        Title = title ?? Title;
        Authors = authors ?? Authors;
        Publisher = publisher ?? Publisher;
        Ranking = ranking ?? Ranking;
        Abstract = abstractText ?? Abstract;
        Doi = doi ?? Doi;
        Url = url ?? Url;
        IsIngested = isIngested ?? IsIngested;
        IsAutoTagged = isAutoTagged ?? IsAutoTagged;
        PublicationDate = publicationDate ?? PublicationDate;
        PaperType = paperType ?? PaperType;
        Pages = pages ?? Pages;
        Number = number ?? Number;
        Volume = volume ?? Volume;
        ConferenceJournalId = conferenceJournalId ?? ConferenceJournalId;
        ReferenceContent = referenceContent ?? ReferenceContent;
        IngestStatus = ingestStatus ?? IngestStatus;
        Keywords = keywords ?? Keywords;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    public void UpdateIngestionStatus(bool isIngested, IngestStatus ingestStatus)
    {
        IsIngested = isIngested;
        IngestStatus = ingestStatus;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    public void UpdateFilePath(string? pdfUrl = null, string? bibUrl = null)
    {
        if (string.IsNullOrWhiteSpace(pdfUrl) && string.IsNullOrWhiteSpace(bibUrl)) return;
        FilePath = pdfUrl ?? FilePath;
        BibFilePath = bibUrl ?? BibFilePath;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    #endregion
}