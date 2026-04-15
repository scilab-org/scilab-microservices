using AutoMapper;
using Management.Application.Dtos.Members;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Member.Queries.GetMemberById;

public record GetMemberByIdQuery(Guid MemberId) : IQuery<MemberDto>;

public class GetMemberByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetMemberByIdQuery, MemberDto>
{
    public async Task<MemberDto> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var member = await session.LoadAsync<MemberEntity>(request.MemberId, cancellationToken);
        if (member == null)
            return new MemberDto();

        return mapper.Map<MemberDto>(member);
    }
}
