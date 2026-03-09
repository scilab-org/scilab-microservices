using Lab.Application.Dtos.Sections;

namespace Lab.Application.Models.Results;

public class GetSectionsByPaperIdResult
{
    public Guid PaperId { get; init; }
    public List<SectionDto> Items { get; init; }

    public GetSectionsByPaperIdResult(Guid paperId, List<SectionDto> items)
    {
        PaperId = paperId;
        Items = items;
    }
}

