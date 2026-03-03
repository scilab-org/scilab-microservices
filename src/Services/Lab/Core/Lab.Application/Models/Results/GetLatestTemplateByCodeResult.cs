using Lab.Application.Dtos.Template;

namespace Lab.Application.Models.Results;

public class GetLatestTemplateByCodeResult
{
    public TemplateDto Template { get; init; }

    public GetLatestTemplateByCodeResult(TemplateDto template)
    {
        Template = template;
    }
}

