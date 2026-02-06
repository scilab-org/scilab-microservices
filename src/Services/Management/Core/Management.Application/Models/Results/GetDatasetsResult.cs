using Management.Application.Dtos.Datasets;

namespace Management.Application.Models.Results;

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