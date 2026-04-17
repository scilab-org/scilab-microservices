using AutoMapper;
using Lab.Application.Dtos.Journals;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Journal.Queries.GetJournalById;

public record GetJournalInProjectByIdQuery(Guid Id, Guid ProjectId) : ICommand<GetJournalByIdResult>;

public class GetJournalInProjectByIdQueryValidator : AbstractValidator<GetJournalInProjectByIdQuery>
{
    public GetJournalInProjectByIdQueryValidator()
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

public class GetJournalInProjectByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetJournalInProjectByIdQuery, GetJournalByIdResult>
{
    #region Implementations

    public async Task<GetJournalByIdResult> Handle(GetJournalInProjectByIdQuery request, CancellationToken cancellationToken)
    {
        // var journal = await session.Query<ConferenceJournalEntity>()
        //     .FirstOrDefaultAsync(x => x.Id == request.Id && x.ProjectId == request.ProjectId, cancellationToken);
        //
        // if (journal == null)
        //     throw new NotFoundException(MessageCode.JournalIsNotExists, request.Id.ToString());
        //
        // var response = mapper.Map<JournalDto>(journal);
        //
        // var templateDtos = new List<TemplateDto>();
        // if (journal.TemplateId != Guid.Empty)
        // {
        //     var template = await session.LoadAsync<TemplateEntity>(journal.TemplateId, cancellationToken);
        //     if (template != null)
        //         templateDtos.Add(mapper.Map<TemplateDto>(template));
        // }
        //
        // return new GetJournalByIdResult(response, templateDtos);
        var response = new GetJournalByIdResult(new JournalDto(), new List<ProjectJournalInfo>(), new List<PaperJournalInfo>());
        return response;
    }

    #endregion
}