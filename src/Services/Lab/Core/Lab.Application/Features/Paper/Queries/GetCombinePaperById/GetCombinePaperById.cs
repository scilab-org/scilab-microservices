using Lab.Application.Dtos.Papers;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using MediatR;

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
                CreatedOnUtc = version.CreatedOnUtc,
                LastModifiedBy = version.LastModifiedBy,
                LastModifiedOnUtc = version.LastModifiedOnUtc ?? version.CreatedOnUtc
            }
        };
    }
}