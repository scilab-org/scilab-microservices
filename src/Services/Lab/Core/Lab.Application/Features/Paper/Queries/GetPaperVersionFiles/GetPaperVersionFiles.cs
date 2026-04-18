using AutoMapper;
using Lab.Application.Dtos.Papers;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Paper.Queries.GetPaperVersionFiles;

public sealed class GetPaperVersionFilesResult
{
    public List<PaperVersionFileDto> Items { get; init; } = [];
}

public record GetPaperVersionFilesQuery(Guid PaperId, Guid VersionId) : ICommand<GetPaperVersionFilesResult>;

public class GetPaperVersionFilesQueryValidator : AbstractValidator<GetPaperVersionFilesQuery>
{
    public GetPaperVersionFilesQueryValidator()
    {
        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);

        RuleFor(x => x.VersionId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperVersionIdIsRequired);
    }
}

public class GetPaperVersionFilesQueryHandler(IDocumentSession session, IMapper mapper)
    : ICommandHandler<GetPaperVersionFilesQuery, GetPaperVersionFilesResult>
{
    public async Task<GetPaperVersionFilesResult> Handle(
        GetPaperVersionFilesQuery request,
        CancellationToken cancellationToken)
    {
        var version = await session.LoadAsync<PaperVersionEntity>(request.VersionId, cancellationToken)
                      ?? throw new NotFoundException(MessageCode.PaperVersionNotFound, request.VersionId.ToString());

        if (version.PaperId != request.PaperId)
            throw new ClientValidationException(
                MessageCode.PaperVersionNotBelongToPaper,
                request.VersionId.ToString());

        var pdfs = await session.Query<PaperVersionFileEntity>()
            .Where(p => p.PaperVersionId == request.VersionId)
            .OrderByDescending(p => p.CreatedOnUtc)
            .ToListAsync(cancellationToken);

        return new GetPaperVersionFilesResult
        {
            Items = mapper.Map<List<PaperVersionFileDto>>(pdfs)
        };
    }
}
