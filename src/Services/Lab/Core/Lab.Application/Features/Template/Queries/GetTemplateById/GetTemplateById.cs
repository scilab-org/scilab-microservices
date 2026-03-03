using AutoMapper;
using Lab.Application.Dtos.Template;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Template.Queries.GetTemplateById;

public record GetTemplateByIdQuery(Guid Id) : IQuery<GetTemplateByIdResult>;

public class GetTemplateByIdQueryValidator : AbstractValidator<GetTemplateByIdQuery>
{
    public GetTemplateByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Template Id is required.");
    }
}

public class GetTemplateByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetTemplateByIdQuery, GetTemplateByIdResult>
{
    #region Implementations

    public async Task<GetTemplateByIdResult> Handle(GetTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        var template = await session.LoadAsync<TemplateEntity>(request.Id, cancellationToken);

        if (template is null)
            throw new NotFoundException($"Template with id {request.Id} not found.");

        var dto = mapper.Map<TemplateDto>(template);

        return new GetTemplateByIdResult(dto);
    }

    #endregion
}

