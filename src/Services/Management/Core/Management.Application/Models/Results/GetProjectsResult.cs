using Management.Application.Dtos.Projects;

using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Models.Results;

[ExcludeFromCodeCoverage]
public sealed class GetProjectsResult
{
    #region Fields, Properties and Indexers

    public List<ProjectDto> Items { get; init; }

    public PagingResult Paging { get; init; }

    #endregion

    #region Ctors

    public GetProjectsResult(
        List<ProjectDto> items,
        long totalCount,
        PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }

    #endregion
}