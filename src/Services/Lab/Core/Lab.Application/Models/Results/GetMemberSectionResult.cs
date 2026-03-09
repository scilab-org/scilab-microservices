using Lab.Application.Dtos.PaperContributors;

namespace Lab.Application.Models.Results;

public class GetMemberSectionResult
{
    public Guid SectionId { get; init; }
    public List<SectionMemberDto> Items { get; init; }

    public GetMemberSectionResult(Guid sectionId, List<SectionMemberDto> items)
    {
        SectionId = sectionId;
        Items = items;
    }
}

public class GetAvailableMemberSectionResult
{
    public Guid SectionId { get; init; }
    public Guid PaperId { get; init; }
    public List<AvailableSectionMemberDto> Items { get; init; }

    public GetAvailableMemberSectionResult(Guid sectionId, Guid paperId, List<AvailableSectionMemberDto> items)
    {
        SectionId = sectionId;
        PaperId = paperId;
        Items = items;
    }
}

