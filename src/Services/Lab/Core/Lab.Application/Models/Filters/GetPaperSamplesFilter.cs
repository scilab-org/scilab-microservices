using Lab.Domain.Enums;

namespace Lab.Application.Models.Filters;

public class GetPaperSamplesFilter
{
    public string? Title { get; set; } = null!;
}