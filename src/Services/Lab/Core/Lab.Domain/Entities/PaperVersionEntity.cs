using Lab.Domain.Abstractions;

namespace Lab.Domain.Entities;

public class PaperVersionEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public Guid PaperId { get; set; }
    public string? Name { get; set; }
    public string? Content { get; set; }
    public List<Guid>? References { get; set; }
    public List<string>? Files { get; set; }

    #endregion

    #region Factories

    public static PaperVersionEntity Create(Guid id,
        Guid paperId,
        string? name,
        string? content,
        List<Guid>? references = null,
        List<string>? files = null,
        string? createdBy = null)
    {
        return new PaperVersionEntity
        {
            Id = id,
            PaperId = paperId,
            Name = name,
            Content = content,
            References = references,
            Files = files,
            CreatedBy = createdBy,
            LastModifiedBy = createdBy,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow
        };
    }

    #endregion

    #region Methods

    public void Update(string? content = null,
        List<Guid>? references = null,
        List<string>? files = null,
        string? lastModifiedBy = null,
        Guid? paperId = null)
    {
        Content = content ?? Content;
        References = references ?? References;
        Files = files ?? Files;
        PaperId = paperId ?? PaperId;
        LastModifiedBy = lastModifiedBy ?? LastModifiedBy;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    #endregion
}