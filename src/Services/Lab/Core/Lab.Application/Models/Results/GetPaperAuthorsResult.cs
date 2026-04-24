using Lab.Application.Dtos.PaperAuthors;

namespace Lab.Application.Models.Results;

public sealed class GetPaperAuthorsResult
{
    public List<PaperAuthorDto> Items { get; init; }
    public PagingResult Paging { get; init; }

    public GetPaperAuthorsResult(List<PaperAuthorDto> items, long totalCount, PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }
}
