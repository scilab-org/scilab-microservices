using AutoMapper;
using Lab.Application.Dtos.PaperAuthors;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.PaperAuthor.Queries.GetPaperAuthorById;

public record GetPaperAuthorByIdQuery(Guid Id) : ICommand<GetPaperAuthorByIdResult>;

public class GetPaperAuthorByIdQueryValidator : AbstractValidator<GetPaperAuthorByIdQuery>
{
    public GetPaperAuthorByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class GetPaperAuthorByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetPaperAuthorByIdQuery, GetPaperAuthorByIdResult>
{
    public async Task<GetPaperAuthorByIdResult> Handle(GetPaperAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<PaperAuthorEntity>(request.Id, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.NotFound, request.Id.ToString());

        var result = mapper.Map<PaperAuthorDto>(entity);
        var role = await session.LoadAsync<AuthorRoleEntity>(entity.AuthorRoleId, cancellationToken);
        result.AuthorRoleName = role?.Name;
        result.AuthorRoleDescription = role?.Description;

        return new GetPaperAuthorByIdResult(result);
    }
}
