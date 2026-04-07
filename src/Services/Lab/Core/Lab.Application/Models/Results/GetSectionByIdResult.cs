using Lab.Application.Dtos.Sections;

namespace Lab.Application.Models.Results;

public class GetSectionByIdResult
{
    public SectionDto Section { get; init; }

    public GetSectionByIdResult(SectionDto section)
    {
        Section = section;
    }
}