using Management.Application.Dtos.Papers;

using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Models.Results;

[ExcludeFromCodeCoverage]
public sealed class GetSubProjectsPapersResult
{
    #region Fields, Properties and Indexers

    public List<PaperInfoDto> Items { get; init; }
    public PagingResult Paging { get; init; }

    #endregion

    #region Ctors

    public GetSubProjectsPapersResult(
        List<PaperInfoDto> items,
        long totalCount,
        PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }

    #endregion
}
