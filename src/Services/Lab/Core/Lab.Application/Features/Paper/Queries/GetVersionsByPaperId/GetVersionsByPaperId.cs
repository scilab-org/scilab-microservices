using Lab.Application.Dtos.Papers;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Paper.Queries.GetVersionsByPaperId;

public record GetVersionsByPaperIdQuery(Guid PaperId) : IQuery<GetVersionsByPaperIdResult>;

public sealed class GetVersionsByPaperIdQueryValidator : AbstractValidator<GetVersionsByPaperIdQuery>
{
    public GetVersionsByPaperIdQueryValidator()
    {
        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);
    }
}

public sealed class GetVersionsByPaperIdQueryHandler(IDocumentSession session)
    : IQueryHandler<GetVersionsByPaperIdQuery, GetVersionsByPaperIdResult>
{
    public async Task<GetVersionsByPaperIdResult> Handle(
        GetVersionsByPaperIdQuery request,
        CancellationToken cancellationToken)
    {
        var paperExists = await session.LoadAsync<PaperEntity>(request.PaperId, cancellationToken)
                          ?? throw new NotFoundException(MessageCode.PaperIsNotExists, request.PaperId.ToString());

        var items = await session.Query<PaperVersionEntity>()
            .Where(x => x.PaperId == paperExists.Id)
            .OrderByDescending(x => x.CreatedOnUtc)
            .Select(version => new PaperVersionInfo
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
            })
            .ToListAsync(cancellationToken);

        return new GetVersionsByPaperIdResult(items.ToList());
    }
}