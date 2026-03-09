using Management.Application.Dtos.Members;

namespace Management.Application.Models.Results;

public sealed class GetSubProjectMembersByPaperIdResult
{
    public Guid SubProjectId { get; init; }
    public List<SubProjectMemberItemDto> Items { get; init; }

    public GetSubProjectMembersByPaperIdResult(Guid subProjectId, List<SubProjectMemberItemDto> items)
    {
        SubProjectId = subProjectId;
        Items = items;
    }
}

