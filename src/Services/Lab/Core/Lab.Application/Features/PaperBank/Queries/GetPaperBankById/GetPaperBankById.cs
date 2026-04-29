using AutoMapper;
using Lab.Application.Dtos.GapTypes;
using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.PaperBank.Queries.GetPaperBankById;

public record GetPaperBankByIdQuery(Guid Id) : ICommand<GetPaperBankByIdResult>;

public class GetPaperBankByIdQueryValidator : AbstractValidator<GetPaperBankByIdQuery>
{
    public GetPaperBankByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);
    }
}

public class GetPaperBankByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetPaperBankByIdQuery, GetPaperBankByIdResult>
{
    public async Task<GetPaperBankByIdResult> Handle(GetPaperBankByIdQuery request, CancellationToken cancellationToken)
    {
        var paper = await session.LoadAsync<PaperBankEntity>(request.Id, cancellationToken);

        if (paper == null)
            throw new NotFoundException(MessageCode.PaperIsNotExists, request.Id.ToString());

        var response = mapper.Map<PaperBankDto>(paper);

        if (paper.ConferenceJournalId.HasValue)
        {
            var journal = await session.LoadAsync<ConferenceJournalEntity>(paper.ConferenceJournalId.Value, cancellationToken);
            if (journal != null)
            {
                response.ConferenceJournalName = journal.Name;
                response.ConferenceJournalType = journal.Type;
            }
        }

        var gapTypeIds = paper.GapTypeIds ?? new List<Guid>();
        if (gapTypeIds.Count > 0)
        {
            var gapTypes = await session.Query<GapTypeEntity>()
                .Where(x => gapTypeIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            response.GapTypes = gapTypes
                .Select(x => new GapTypeInfoDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList();
        }

        return new GetPaperBankByIdResult(response);
    }
}