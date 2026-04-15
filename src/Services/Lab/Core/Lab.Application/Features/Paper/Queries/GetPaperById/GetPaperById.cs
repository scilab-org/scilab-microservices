using AutoMapper;
using Lab.Application.Dtos.Papers;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Paper.Queries.GetPaperById;


public record GetPaperByIdQuery(Guid Id) : ICommand<GetPaperByIdResult>;

public class GetPaperByIdQueryValidator : AbstractValidator<GetPaperByIdQuery>
{
    public GetPaperByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);
    }
}

public class GetPaperByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetPaperByIdQuery, GetPaperByIdResult>
{
    public async Task<GetPaperByIdResult> Handle(GetPaperByIdQuery request, CancellationToken cancellationToken)
    {
        var paper = await session.LoadAsync<PaperEntity>(request.Id, cancellationToken);

        if (paper == null)
            throw new NotFoundException(MessageCode.PaperIsNotExists, request.Id.ToString());

        var response = mapper.Map<PaperDto>(paper);
        var versions = await session.Query<PaperVersionEntity>()
            .Where(x => x.PaperId == paper.Id)
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

        response.Versions = versions.ToList();

        return new GetPaperByIdResult(response);
    }
}