using Lab.Domain.Abstractions;

namespace Lab.Domain.Entities;

public sealed class PaperVersionFileEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public Guid PaperVersionId { get; set; }
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public string? Note { get; set; }

    #endregion

    #region Factories

    public static PaperVersionFileEntity Create(
        Guid paperVersionId,
        string fileName,
        string fileUrl,
        string? note = null,
        string? createdBy = null)
    {
        return new PaperVersionFileEntity
        {
            Id = Guid.NewGuid(),
            PaperVersionId = paperVersionId,
            FileName = fileName,
            FileUrl = fileUrl,
            Note = note,
            CreatedBy = createdBy,
            LastModifiedBy = createdBy,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow
        };
    }

    #endregion
}
