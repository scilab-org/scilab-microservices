using Lab.Application.Dtos.Template;

namespace Lab.Application.Models.Results;

public class GetTemplateVersionsResult
{
    public List<TemplateDto> Versions { get; init; }

    public GetTemplateVersionsResult(List<TemplateDto> versions)
    {
        Versions = versions;
    }
}

