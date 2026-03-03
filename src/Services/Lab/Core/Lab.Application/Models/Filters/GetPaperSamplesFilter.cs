using Lab.Domain.Enums;

namespace Lab.Application.Models.Filters;

public class GetPaperSamplesFilter
{
    public string? Title { get; set; } = null!;
    public PaperStatus? Status { get; set; }
}