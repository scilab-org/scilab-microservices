using AutoMapper;
using Lab.Application.Dtos.Papers;
using Lab.Application.Dtos.Tags;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Tag.Queries.GetTagById;

public record GetTagByIdQuery(Guid Id) : ICommand<GetTagByIdResult>;

public class GetTagByIdQueryValidator : AbstractValidator<GetTagByIdQuery>
{
    public GetTagByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage(MessageCode.TagIdIsRequired)
            .NotEmpty()
            .WithMessage(MessageCode.TagIdIsRequired);
    }
}

public class GetTagByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetTagByIdQuery, GetTagByIdResult>
{
    #region Implementations

    public async Task<GetTagByIdResult> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        var tag = await session.LoadAsync<TagEntity>(request.Id, cancellationToken);

        if (tag == null)
            throw new NotFoundException(MessageCode.TagIsNotExists, request.Id.ToString());

        var response = mapper.Map<TagDto>(tag);

        return new GetTagByIdResult(response);
    }
}

#endregion