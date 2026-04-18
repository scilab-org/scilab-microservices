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

        var dtos = mapper.Map<List<PaperStatusHistoryDto>>(history);

        // Populate PDF info for entries that reference a PDF file
        var pdfFileIds = history
            .Where(h => h.PdfFileId.HasValue)
            .Select(h => h.PdfFileId!.Value)
            .Distinct()
            .ToList();

        if (pdfFileIds.Count > 0)
        {
            var pdfFiles = await session.Query<PaperVersionFileEntity>()
                .Where(p => p.Id.IsOneOf(pdfFileIds))
                .ToListAsync(cancellationToken);

            var pdfLookup = pdfFiles.ToDictionary(p => p.Id);

            foreach (var dto in dtos)
            {
                if (dto.PdfFileId.HasValue && pdfLookup.TryGetValue(dto.PdfFileId.Value, out var pdf))
                {
                    dto.PdfFileName = pdf.FileName;
                    dto.PdfFileUrl = pdf.FileUrl;
                }
            }
        }

        return new GetPaperStatusHistoryResult
        {
            PaperId = paper.Id,
            CurrentStatus = currentStatus,
            History = dtos
        };
    }

    #endregion
}
