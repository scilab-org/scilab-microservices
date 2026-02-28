using Management.Application.Dtos.Papers;

namespace Management.Application.Models.Results;

public sealed class GetProjectPapersResult
{
    #region Fields, Properties and Indexers

    public List<PaperInfoDto> Items { get; init; }
    public int TotalCount { get; init; }

    #endregion

    #region Ctors

    public GetProjectPapersResult(List<PaperInfoDto> items)
    {
        Items = items;
        TotalCount = items.Count;
    }

    #endregion
}
