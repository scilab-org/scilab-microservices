using Lab.Application.Dtos.Journals;

namespace Lab.Application.Models.Results;

public sealed class GetJournalsResult
{
    #region Fields, Properties and Indexers

    public List<JournalDto> Items { get; init; }

    public PagingResult Paging { get; init; }

    #endregion

    #region Ctors

    public GetJournalsResult(
        List<JournalDto> items,
        long totalCount,
        PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }

    #endregion
}