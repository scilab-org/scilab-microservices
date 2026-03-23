using Lab.Application.Dtos.Abstractions;
using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Papers;

public  class PaperInfoDto : DtoId<Guid>
{
    public string Title { get; set; } = null!;
    public string? Template { get; set; }
    public string? FilePath { get; set; }
    public string? Context { get; set; }
    public string? Abstract { get; set; }
    public string? ResearchGap { get; set; }
    public string? MainContribution { get; set; }
    public string? Rule { get; set; }
    public string? GapType { get; set; }
    public string? Journal { get; set; }
    public string? StyleName { get; set; }
    public string? StyleDescription { get; set; }
    public string? StyleRule { get; set; }
    public PaperStatus? Status { get; set; }
    public List<string> TagNames { get; set; } = new();
}