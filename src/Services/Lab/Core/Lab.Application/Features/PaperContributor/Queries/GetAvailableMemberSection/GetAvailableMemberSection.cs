using Lab.Application.Dtos.PaperContributors;
using Lab.Application.Models.Results;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.PaperContributor.Queries.GetAvailableMemberSection;

public record GetAvailableMemberSectionQuery(Guid SectionId, Guid PaperId) : IQuery<GetAvailableMemberSectionResult>;

public sealed class GetAvailableMemberSectionQueryValidator : AbstractValidator<GetAvailableMemberSectionQuery>
{
    public GetAvailableMemberSectionQueryValidator()
    {
        RuleFor(x => x.SectionId)
            .NotEmpty()
            .WithMessage(MessageCode.SectionIdIsRequired);

        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);
    }
}

public sealed class GetAvailableMemberSectionQueryHandler(
    IDocumentSession session,
    IManagementApiService managementApiService)
    : IQueryHandler<GetAvailableMemberSectionQuery, GetAvailableMemberSectionResult>
{
    public async Task<GetAvailableMemberSectionResult> Handle(
        GetAvailableMemberSectionQuery request,
        CancellationToken cancellationToken)
    {
        // MemberIds already assigned to this section
        var assignedMemberIds = (await session.Query<PaperContributorEntity>()
                .Where(c => c.MarkSectionId == request.SectionId)
                .Select(c => c.MemberId)
                .ToListAsync(cancellationToken))
            .ToHashSet();

        // All sub-project members with user info
        var allMembers = await managementApiService.GetSubProjectMembersByPaperIdAsync(
            request.PaperId, cancellationToken);

        // Filter out already-assigned, map user info
        var available = allMembers
            .Where(m => !assignedMemberIds.Contains(m.MemberId))
            .Select(m => new AvailableSectionMemberDto
            {
                MemberId  = m.MemberId,
                UserId    = m.UserId,
                Role      = m.Role,
                Username  = m.Username,
                Email     = m.Email,
                FirstName = m.FirstName,
                LastName  = m.LastName
            })
            .ToList();

        return new GetAvailableMemberSectionResult(request.SectionId, request.PaperId, available);
    }
}
