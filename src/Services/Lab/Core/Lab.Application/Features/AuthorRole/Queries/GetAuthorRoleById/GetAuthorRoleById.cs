using AutoMapper;
using Lab.Application.Dtos.AuthorRoles;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.AuthorRole.Queries.GetAuthorRoleById;

public record GetAuthorRoleByIdQuery(Guid Id) : ICommand<GetAuthorRoleByIdResult>;

public class GetAuthorRoleByIdQueryValidator : AbstractValidator<GetAuthorRoleByIdQuery>
{
    public GetAuthorRoleByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(MessageCode.RoleNotFound);
    }
}

public class GetAuthorRoleByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetAuthorRoleByIdQuery, GetAuthorRoleByIdResult>
{
    public async Task<GetAuthorRoleByIdResult> Handle(GetAuthorRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<AuthorRoleEntity>(request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(MessageCode.RoleNotFound, request.Id.ToString());

        return new GetAuthorRoleByIdResult(mapper.Map<AuthorRoleDto>(entity));
    }
}
