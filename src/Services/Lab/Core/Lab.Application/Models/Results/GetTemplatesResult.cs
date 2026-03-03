using Lab.Application.Dtos.Template;

namespace Lab.Application.Models.Results;

public class GetTemplatesResult
{
    public List<TemplateDto> Items { get; init; }
    public PagingResult Paging { get; init; }

    public GetTemplatesResult(List<TemplateDto> items, long totalCount, PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }
}
