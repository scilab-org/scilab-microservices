using Lab.Application.Dtos.Papers;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Paper.Queries.GetCombinePaperById;

public record GetCombinePaperByIdQuery(Guid PaperId, Guid VersionId) : IRequest<CombineSectionsToPaperResult>;

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

public class GetCombinePaperByIdQueryHandler(IDocumentSession session) : IRequestHandler<GetCombinePaperByIdQuery, CombineSectionsToPaperResult>
{
    public async Task<CombineSectionsToPaperResult> Handle(GetCombinePaperByIdQuery request, CancellationToken cancellationToken)
    {
        var paper = await session.LoadAsync<PaperEntity>(request.PaperId, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.PaperIsNotExists, request.PaperId.ToString());

        var combine = paper.Combines.FirstOrDefault(x => x.Id == request.VersionId)
                      ?? throw new NotFoundException(MessageCode.PaperCombineIsNotExists, request.VersionId.ToString());

        return new CombineSectionsToPaperResult
        {
            Combine = new PaperCombineInfo()
            {
                Id = combine.Id,
                Name = combine.Name,
                Content = combine.Content,
                References = combine.References,
                IsSave = true,
                CreatedBy = combine.CreatedBy,
                CreatedOnUtc = combine.CreatedOnUtc,
                LastModifiedBy = combine.LastModifiedBy,
                LastModifiedOnUtc = combine.LastModifiedOnUtc
            }
        };
    }
}