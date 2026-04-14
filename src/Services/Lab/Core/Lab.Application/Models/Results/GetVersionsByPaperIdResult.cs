using Lab.Application.Dtos.Papers;

namespace Lab.Application.Models.Results;

public class GetVersionsByPaperIdResult
{
    public List<PaperVersionInfo> Items { get; init; }

    public GetVersionsByPaperIdResult(List<PaperVersionInfo> items)
    {
        Items = items;
    }
}