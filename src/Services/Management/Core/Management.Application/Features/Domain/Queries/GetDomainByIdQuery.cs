using AutoMapper;
using Management.Application.Dtos.Domains;
using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.Domain.Queries;

public sealed record GetDomainByIdQuery(Guid DomainId) : IRequest<DomainDto>;

public sealed class GetDomainByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetDomainByIdQuery, DomainDto>
{
    public async Task<DomainDto> Handle(GetDomainByIdQuery query, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<DomainEntity>(query.DomainId, cancellationToken)
            ?? throw new ClientValidationException(MessageCode.DomainIsNotExists, query.DomainId.ToString());

        return mapper.Map<DomainDto>(entity);
    }
}
