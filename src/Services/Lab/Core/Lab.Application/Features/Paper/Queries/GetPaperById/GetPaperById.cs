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

public class GetPaperByIdQueryHandler(IDocumentSession session, IMapper mapper) : IRequestHandler<GetPaperByIdQuery, GetPaperByIdResult>
{
    public async Task<GetPaperByIdResult> Handle(GetPaperByIdQuery request, CancellationToken cancellationToken)
    {
        var paper = await session.LoadAsync<PaperEntity>(request.Id, cancellationToken);

        if (paper == null)
            throw new NotFoundException(MessageCode.PaperIsNotExists, request.Id.ToString());

        var response = mapper.Map<PaperDto>(paper);

        return new GetPaperByIdResult(response);
    }
}