using Lab.Domain.Abstractions;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public class ConferenceJournalEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string Name { get; set; } = null!;
    public string? Ranking { get; set; }
    public string? Url { get; set; }
    public string? ISSN { get; set; }
    public string? TexFile { get; set; }
    public string? PdfFile { get; set; }
    public string? Style { get; set; }
    public ConferenceJournalType Type { get; set; }
    public List<Guid>? TemplateIds { get; set; }
    public List<Guid>? ProjectIds { get; set; }
    public List<Guid>? PaperIds { get; set; }

    #endregion

    #region Factories

    public static ConferenceJournalEntity Create(
        Guid id,
        string name,
        string? ranking,
        string? url,
        string? issn,
        string? style,
        ConferenceJournalType type,
        List<Guid> templateIds,
        string? texFile,
        string? pdfFile,
        List<Guid>? projectIds = null,
        List<Guid>? paperIds = null,
        string? createdBy = null)
    {
        return new ConferenceJournalEntity()
        {
            Id = id,
            Name = name,
            Ranking = ranking,
            Url = url,
            ISSN = issn,
            ProjectIds = projectIds ?? [],
            PaperIds = paperIds ?? [],
            Style = style,
            Type = type,
            TemplateIds = templateIds,
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
        string? ranking = null,
        string? url = null,
        string? issn = null,
        List<Guid>? projectIds = null,
        List<Guid>? paperIds = null,
        string? style = null,
        ConferenceJournalType? type = null,
        List<Guid>? templateIds = null,
        string? texFile = null,
        string? pdfFile = null,
        string? lastModifiedBy = null)
    {
        Name = name ?? Name;
        Ranking = ranking ?? Ranking;
        Url = url ?? Url;
        ISSN = issn ?? ISSN;
        ProjectIds = projectIds ?? ProjectIds;
        PaperIds = paperIds ?? PaperIds;
        Style = style ?? Style;
        Type = type ?? Type;
        TemplateIds = templateIds ?? TemplateIds;
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