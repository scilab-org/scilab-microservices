using Management.Application.Dtos.Papers;

namespace Management.Application.Models.Results;

public sealed class GetAvailablePapersResult
{
    #region Fields, Properties and Indexers

    public List<PaperInfoDto> Items { get; init; }
    public int TotalCount { get; init; }

    #endregion

    #region Ctors

    public GetAvailablePapersResult(List<PaperInfoDto> items)
    {
        Items = items;
        TotalCount = items.Count;
    }

    #endregion
}
