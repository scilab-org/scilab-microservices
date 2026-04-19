using Management.Application.Dtos.Papers;

using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Models.Results;

[ExcludeFromCodeCoverage]
public sealed class GetAvailablePapersResult
{
    #region Fields, Properties and Indexers

    public List<PaperBankInfoDto> Items { get; init; }
    public PagingResult Paging { get; init; }

    #endregion

    #region Ctors

    public GetAvailablePapersResult(
        List<PaperBankInfoDto> items,
        long totalCount,
        PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }

    #endregion
}
