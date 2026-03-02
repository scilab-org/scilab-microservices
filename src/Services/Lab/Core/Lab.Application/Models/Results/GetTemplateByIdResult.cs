using Lab.Application.Dtos.Template;

namespace Lab.Application.Models.Results;

public class GetTemplateByIdResult
{
    public TemplateDto Template { get; init; }

    public GetTemplateByIdResult(TemplateDto template)
    {
        Template = template;
    }
}

