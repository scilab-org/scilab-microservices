using Lab.Domain.Abstractions;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public sealed class PaperEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string Title { get; set; } = null!;
    public string? Template { get; set; }
    public string? FilePath { get; set; }
    public string? Context { get; set; }
    public PaperStatus? Status { get; set; }
    public string? PaperType { get; set; }
    public List<string> TagNames { get; set; } = new();

    #endregion

    #region Factories

    public static PaperEntity Create(Guid id,
        string title,
        string? template = null,
        string? context = null,
        PaperStatus? status = null,
        string? paperType = null,
        List<string>? tagNames = null)
    {
        return new PaperEntity()
        {
            Id = id,
            Title = title,
            Template = template,
            Context = context,
            Status = status ?? PaperStatus.Processing,
            PaperType = paperType,
            TagNames = tagNames ?? new(),
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }

    #endregion

    #region Methods

    public void Update(string? title = null,
        string? template = null,
        string? context = null,
        PaperStatus? status = null,
        string? paperType = null,
        List<string>? tagNames = null)
    {
        Title = title ?? Title;
        Template = template ?? Template;
        Context = context ?? Context;
        Status = status ?? Status;
        PaperType = paperType  ?? PaperType;
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