using Lab.Application.Dtos.Sections;

namespace Lab.Application.Models.Results;

public class GetSectionVersionsByMarkSectionIdResult
{
    public List<SectionDto> Items { get; init; }

    public GetSectionVersionsByMarkSectionIdResult(List<SectionDto> items)
    {
        Items = items;
    }
}
