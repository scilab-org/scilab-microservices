using AutoMapper;
using Lab.Application.Dtos.Template;
using Lab.Application.Dtos.Journals;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Journal.Queries.GetJournalById;

public record GetJournalByIdQuery(Guid Id, Guid ProjectId) : ICommand<GetJournalByIdResult>;

public class GetJournalByIdQueryValidator : AbstractValidator<GetJournalByIdQuery>
{
    public GetJournalByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage(MessageCode.JournalIdIsRequired)
            .NotEmpty()
            .WithMessage(MessageCode.JournalIdIsRequired);

        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.JournalProjectIdIsRequired);
    }
}

public class GetJournalByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetJournalByIdQuery, GetJournalByIdResult>
{
    #region Implementations

    public async Task<GetJournalByIdResult> Handle(GetJournalByIdQuery request, CancellationToken cancellationToken)
    {
        var journal = await session.Query<ConferenceJournalEntity>()
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.ProjectId == request.ProjectId, cancellationToken);

        if (journal == null)
            throw new NotFoundException(MessageCode.JournalIsNotExists, request.Id.ToString());

        var response = mapper.Map<JournalDto>(journal);

        var templates = await session.Query<TemplateEntity>()
            .Where(t => t.ConferenceJournalId == request.Id)
            .ToListAsync(cancellationToken);
        var templateDtos = mapper.Map<List<TemplateDto>>(templates);

        return new GetJournalByIdResult(response, templateDtos);
    }

    #endregion
}