﻿using Management.Application.Dtos.Members;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Member.Queries;

public record GetMemberByPaperIdQuery(Guid PaperId, Guid UserId) : IQuery<ProjectMemberDto>;

public sealed class GetMemberByPaperIdQueryValidator : AbstractValidator<GetMemberByPaperIdQuery>
{
    public GetMemberByPaperIdQueryValidator()
    {
        RuleFor(x => x.PaperId).NotEmpty().WithMessage(MessageCode.PaperIdIsRequired);
        RuleFor(x => x.UserId).NotEmpty().WithMessage(MessageCode.UserIdIsRequired);
    }
}

public sealed class GetMemberByPaperIdQueryHandler(IDocumentSession session)
    : IQueryHandler<GetMemberByPaperIdQuery, ProjectMemberDto>
{
    #region Implementations

    public async Task<ProjectMemberDto> Handle(GetMemberByPaperIdQuery request, CancellationToken cancellationToken)
    {
        // Find the sub-project that contains this paper
        var subProject = await session.Query<ProjectEntity>()
            .Where(p => p.PaperIds.Contains(request.PaperId) && p.ParentProjectId != null)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(MessageCode.SubProjectNotFound, request.PaperId.ToString());

        // Find the member record for this user in that sub-project
        var member = await session.Query<MemberEntity>()
            .Where(m => m.UserId == request.UserId && m.ProjectId == subProject.Id)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(MessageCode.MemberNotFound, request.UserId.ToString());

        return new ProjectMemberDto
        {
            MemberId    = member.Id,
            UserId      = member.UserId,
            SubProjectId = subProject.Id,
            ProjectId   = subProject.ParentProjectId ?? Guid.Empty,
            Role        = member.ProjectRole,
            JoinedAt    = member.JoinedAt,
        };
    }

    #endregion
}



