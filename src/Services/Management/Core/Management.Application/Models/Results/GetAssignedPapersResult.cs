using Management.Application.Dtos.Papers;

using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Models.Results;

[ExcludeFromCodeCoverage]
public sealed class GetAssignedPapersResult
{
    public List<AssignedPaperDto> Items { get; init; }
    public PagingResult Paging { get; init; }

    public GetAssignedPapersResult(List<AssignedPaperDto> items, long totalCount, PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }
}
