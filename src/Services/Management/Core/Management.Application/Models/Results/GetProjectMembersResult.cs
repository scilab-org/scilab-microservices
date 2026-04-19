using Management.Application.Dtos.Members;

using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Models.Results;

[ExcludeFromCodeCoverage]
public sealed class GetProjectMembersResult
{
    #region Fields, Properties and Indexers

    public List<ProjectMemberDto> Items { get; init; }

    public PagingResult Paging { get; init; }

    #endregion

    #region Ctors

    public GetProjectMembersResult(
        List<ProjectMemberDto> items,
        long totalCount,
        PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }

    #endregion
}

