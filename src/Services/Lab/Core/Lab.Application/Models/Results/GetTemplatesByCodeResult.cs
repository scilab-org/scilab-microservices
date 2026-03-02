using Lab.Application.Dtos.Template;

namespace Lab.Application.Models.Results;

public class GetTemplatesByCodeResult
{
    public List<TemplateDto> Items { get; init; }

    public GetTemplatesByCodeResult(List<TemplateDto> items)
    {
        Items = items;
    }
}

