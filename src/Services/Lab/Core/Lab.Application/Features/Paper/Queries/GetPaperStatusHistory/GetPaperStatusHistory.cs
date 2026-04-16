using AutoMapper;
using Lab.Application.Dtos.Papers;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;
using MediatR;

namespace Lab.Application.Features.Paper.Queries.GetPaperStatusHistory;

public record GetPaperStatusHistoryQuery(Guid PaperId) : ICommand<GetPaperStatusHistoryResult>;

public class GetPaperStatusHistoryQueryValidator : AbstractValidator<GetPaperStatusHistoryQuery>
{
    public GetPaperStatusHistoryQueryValidator()
    {
        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);
    }
}

public class GetPaperStatusHistoryQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetPaperStatusHistoryQuery, GetPaperStatusHistoryResult>
{
    #region Implementation

    public async Task<GetPaperStatusHistoryResult> Handle(
        GetPaperStatusHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var paper = await session.LoadAsync<PaperEntity>(request.PaperId, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.PaperIsNotExists, request.PaperId.ToString());

        var history = await session.Query<PaperStatusHistoryEntity>()
            .Where(h => h.PaperId == request.PaperId)
            .OrderByDescending(h => h.CreatedOnUtc)
            .ToListAsync(cancellationToken);

        var currentStatus = history.Count > 0
            ? history[0].Status
            : SubmissionStatus.Draft;

        return new GetPaperStatusHistoryResult
        {
            PaperId = paper.Id,
            CurrentStatus = currentStatus,
            History = mapper.Map<List<PaperStatusHistoryDto>>(history)
        };
    }

    #endregion
}
