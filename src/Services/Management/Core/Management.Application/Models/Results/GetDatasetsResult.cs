using Management.Application.Dtos.Datasets;

using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Models.Results;

[ExcludeFromCodeCoverage]
public sealed class GetDatasetsResult
{
    #region Fields, Properties and Indexers

    public List<DatasetDto> Items { get; init; }

    public PagingResult Paging { get; init; }

    #endregion
    
    #region Ctors
    
    public GetDatasetsResult(
        List<DatasetDto> items,
        long totalCount,
        PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }
    
    #endregion
}