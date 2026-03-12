using Lab.Application.Dtos.Sections;

namespace Lab.Application.Models.Results;

public class GetSectionByMarkSectionIdResult
{
    public List<SectionContributorDto> Items { get; init; }

    public GetSectionByMarkSectionIdResult(List<SectionContributorDto> items)
    {
        Items = items;
    }
}