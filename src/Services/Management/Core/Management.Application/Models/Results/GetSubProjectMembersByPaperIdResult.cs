using Management.Application.Dtos.Members;

using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Models.Results;

[ExcludeFromCodeCoverage]
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

