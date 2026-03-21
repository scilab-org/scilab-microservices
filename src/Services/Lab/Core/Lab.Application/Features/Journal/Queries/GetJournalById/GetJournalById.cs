using AutoMapper;
using Lab.Application.Dtos.Journals;
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
    #region Implementations

    public async Task<GetJournalByIdResult> Handle(GetJournalByIdQuery request, CancellationToken cancellationToken)
    {
        var journal = await session.LoadAsync<JournalEntity>(request.Id, cancellationToken);

        if (journal == null)
            throw new NotFoundException(MessageCode.JournalIsNotExists, request.Id.ToString());

        var response = mapper.Map<JournalDto>(journal);

        return new GetJournalByIdResult(response);
    }

    #endregion
}