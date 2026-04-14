using AutoMapper;
using Lab.Application.Dtos.Journals;
using Lab.Application.Dtos.Template;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Journal.Queries.GetJournalById;

public record GetJournalByIdQuery(Guid Id) : ICommand<GetJournalByIdResult>;

public class GetJournalByIdQueryValidator : AbstractValidator<GetJournalByIdQuery>
{
    public GetJournalByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage(MessageCode.JournalIdIsRequired)
            .NotEmpty()
            .WithMessage(MessageCode.JournalIdIsRequired);
    }
}

public class GetJournalByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetJournalByIdQuery, GetJournalByIdResult>
{
    public async Task<GetJournalByIdResult> Handle(GetJournalByIdQuery request, CancellationToken cancellationToken)
    {
        var journal = await session.LoadAsync<ConferenceJournalEntity>(request.Id, cancellationToken);

        if (journal == null)
            throw new NotFoundException(MessageCode.JournalIsNotExists, request.Id.ToString());

        var response = mapper.Map<JournalDto>(journal);

        var templateDtos = new List<TemplateDto>();
        if (journal.TemplateId != Guid.Empty)
        {
            var template = await session.LoadAsync<TemplateEntity>(journal.TemplateId, cancellationToken);
            if (template != null)
                templateDtos.Add(mapper.Map<TemplateDto>(template));
        }

        return new GetJournalByIdResult(response, templateDtos);
    }
}