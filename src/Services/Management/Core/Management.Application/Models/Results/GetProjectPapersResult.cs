using Management.Application.Dtos.Papers;

using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Models.Results;

[ExcludeFromCodeCoverage]
public sealed class GetProjectPapersResult
{
    #region Fields, Properties and Indexers

    public List<PaperBankInfoDto> Items { get; init; }
    public PagingResult Paging { get; init; }

    #endregion

    #region Ctors

    public GetProjectPapersResult(List<PaperBankInfoDto> items,
        long totalCount,
        PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }

    #endregion
}
