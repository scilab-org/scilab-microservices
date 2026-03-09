using Lab.Application.Dtos.PaperContributors;
using Lab.Application.Models.Results;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.PaperContributor.Queries.GetMemberSection;

public record GetMemberSectionQuery(Guid SectionId, Guid PaperId) : IQuery<GetMemberSectionResult>;

public sealed class GetMemberSectionQueryValidator : AbstractValidator<GetMemberSectionQuery>
{
    public GetMemberSectionQueryValidator()
    {
        RuleFor(x => x.SectionId)
            .NotEmpty()
            .WithMessage(MessageCode.SectionIdIsRequired);

        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);
    }
}

public sealed class GetMemberSectionQueryHandler(
    IDocumentSession session,
    IManagementApiService managementApiService)
    : IQueryHandler<GetMemberSectionQuery, GetMemberSectionResult>
{
    public async Task<GetMemberSectionResult> Handle(
        GetMemberSectionQuery request,
        CancellationToken cancellationToken)
    {
        // All contributors assigned to this section
        var contributors = await session.Query<PaperContributorEntity>()
            .Where(c => c.MarkSectionId == request.SectionId)
            .ToListAsync(cancellationToken);

        if (!contributors.Any())
            return new GetMemberSectionResult(request.SectionId, []);

        // Resolve UserId + user info for each MemberId via Management service (single call)
        var allMembers = await managementApiService.GetSubProjectMembersByPaperIdAsync(
            request.PaperId, cancellationToken);

        // memberInfo lookup: MemberId -> SubProjectMemberInfo
        var memberMap = allMembers.ToDictionary(m => m.MemberId);

        // Map to DTO with user info
        var items = contributors.Select(c =>
        {
            memberMap.TryGetValue(c.MemberId, out var m);
            return new SectionMemberDto
            {
                Id                 = c.Id,
                PaperContributorId = c.Id,
                MemberId           = c.MemberId,
                UserId             = m?.UserId ?? Guid.Empty,
                SectionRole        = c.SectionRole,
                MarkSectionId      = c.MarkSectionId,
                SectionId          = c.SectionId,
                Username           = m?.Username,
                Email              = m?.Email,
                FirstName          = m?.FirstName,
                LastName           = m?.LastName
            };
        }).ToList();

        return new GetMemberSectionResult(request.SectionId, items);
    }
}
