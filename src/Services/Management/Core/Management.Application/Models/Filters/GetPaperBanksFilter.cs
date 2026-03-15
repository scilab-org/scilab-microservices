namespace Management.Application.Models.Filters;

public class GetPaperBanksFilter
{
    public string? Title { get; set; } = null!;
    public string[]? Tag { get; set; }
}