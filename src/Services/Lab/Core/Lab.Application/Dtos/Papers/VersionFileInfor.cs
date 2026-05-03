using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Papers;

public class VersionFileInfor
{
    public Guid Id { get; set; }
    public Guid? PaperVersionId { get; set; }
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public SubmissionStatus Status { get; set; }
    public string? Note { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string? CreatedBy { get; set; }
}