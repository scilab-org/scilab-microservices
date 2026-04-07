using AutoMapper;
using Lab.Application.Dtos.Sections;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Section.Queries.GetSectionById;

public record GetSectionByIdQuery(Guid Id) : ICommand<GetSectionByIdResult>;

public class GetSectionByIdQueryValidator : AbstractValidator<GetSectionByIdQuery>
{
    public GetSectionByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.SectionIdIsRequired);
    }
}

public class GetSectionByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetSectionByIdQuery, GetSectionByIdResult>
{
    public async Task<GetSectionByIdResult> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
    {
        var section = await session.LoadAsync<SectionEntity>(request.Id, cancellationToken);

        if (section == null)
            throw new NotFoundException(MessageCode.SectionIsNotExists, request.Id.ToString());

        var response = mapper.Map<SectionDto>(section);

        return new GetSectionByIdResult(response);
    }
}