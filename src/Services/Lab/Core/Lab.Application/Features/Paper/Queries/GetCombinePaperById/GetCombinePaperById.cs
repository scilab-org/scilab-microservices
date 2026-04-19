using Lab.Application.Dtos.Papers;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Paper.Queries.GetCombinePaperById;

public record GetCombinePaperByIdQuery(Guid PaperId, Guid VersionId) : ICommand<CombineSectionsToPaperResult>;

public class GetCombinePaperByIdQueryValidator : AbstractValidator<GetCombinePaperByIdQuery>
{
    public GetCombinePaperByIdQueryValidator()
    {
        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired)
            .NotNull()
            .WithMessage(MessageCode.PaperIdIsRequired);
        RuleFor(x => x.VersionId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperCombineVersionIdIsRequired)
            .NotNull()
            .WithMessage(MessageCode.PaperCombineVersionIdIsRequired);
    }
}

public class GetCombinePaperByIdQueryHandler(IDocumentSession session) : ICommandHandler<GetCombinePaperByIdQuery, CombineSectionsToPaperResult>
{
    public async Task<CombineSectionsToPaperResult> Handle(GetCombinePaperByIdQuery request, CancellationToken cancellationToken)
    {
        _ = await session.LoadAsync<PaperEntity>(request.PaperId, cancellationToken)
            ?? throw new NotFoundException(MessageCode.PaperIsNotExists, request.PaperId.ToString());

        var version = await session.LoadAsync<PaperVersionEntity>(request.VersionId, cancellationToken)
                      ?? throw new NotFoundException(MessageCode.PaperCombineIsNotExists, request.VersionId.ToString());

        var versionFile = await session.Query<PaperVersionFileEntity>()
            .Where(x => x.PaperVersionId == version.Id)
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToListAsync(cancellationToken);

        var versionFileIds = versionFile.Select(x => x.Id).ToHashSet();

        var statusHistory = await session.Query<PaperStatusHistoryEntity>()
            .Where(x => x.PaperId == request.PaperId && x.PdfFileId != null)
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToListAsync(cancellationToken);

        var statusByPdfFileId = statusHistory
            .Where(x => x.PdfFileId.HasValue && versionFileIds.Contains(x.PdfFileId.Value))
            .GroupBy(x => x.PdfFileId!.Value)
            .ToDictionary(x => x.Key, x => x.First().Status);

        var versionFileInfor = versionFile.Select(x => new VersionFileInfor
        {
            Id = x.Id,
            PaperVersionId = x.PaperVersionId,
            FileName = x.FileName,
            FileUrl = x.FileUrl,
            Status = statusByPdfFileId.GetValueOrDefault(x.Id),
            Note = x.Note,
            CreatedBy = x.CreatedBy,
            CreatedOnUtc = x.CreatedOnUtc
        }).ToList();

        return new CombineSectionsToPaperResult
        {
            Version = new PaperVersionInfo()
            {
                Id = version.Id,
                Name = version.Name,
                Content = version.Content,
                References = version.References,
                Files = version.Files,
                CreatedBy = version.CreatedBy,
                VersionFiles = versionFileInfor,
                CreatedOnUtc = version.CreatedOnUtc,
                LastModifiedBy = version.LastModifiedBy,
                LastModifiedOnUtc = version.LastModifiedOnUtc ?? version.CreatedOnUtc
            }
        };
    }
}