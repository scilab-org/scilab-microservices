using Lab.Application.Dtos.Sections;

namespace Lab.Application.Models.Results;

public class GetSectionsByPaperIdResult
{
    public List<SectionDto> Items { get; init; }

    public GetSectionsByPaperIdResult(List<SectionDto> items)
    {
        Items = items;
    }
}

