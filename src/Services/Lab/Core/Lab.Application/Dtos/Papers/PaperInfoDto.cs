using Lab.Application.Dtos.Abstractions;
using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Papers;

public  class PaperInfoDto : DtoId<Guid>
{
    public string Title { get; set; } = null!;
    public string? Template { get; set; }
    public string? FilePath { get; set; }
    public PaperStatus? Status { get; set; }
    public string? Context { get; set; }
    public DateTimeOffset? PublicationDate { get; set; }
    public string? PaperType { get; set; }
    public List<string> TagNames { get; set; } = new();
}