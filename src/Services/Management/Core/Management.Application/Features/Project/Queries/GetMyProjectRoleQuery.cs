using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Queries;

public record GetMyProjectRoleQuery(Guid UserId, Guid ProjectId) : IQuery<string>;

public sealed class GetMyProjectRoleQueryValidator : AbstractValidator<GetMyProjectRoleQuery>
{
    public GetMyProjectRoleQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(MessageCode.UserIdIsRequired);
        RuleFor(x => x.ProjectId)            
            .NotEmpty()
            .WithMessage(MessageCode.ProjectIdIsRequired);
    }
}

public sealed class GetMyProjectRoleQueryHandler(IDocumentSession session) : IQueryHandler<GetMyProjectRoleQuery, string>
{
    #region Implementations

    public async Task<string> Handle(GetMyProjectRoleQuery req, CancellationToken cancellationToken)
    {
        var member = await session.Query<MemberEntity>()
            .FirstOrDefaultAsync(x => x.UserId == req.UserId && x.ProjectId == req.ProjectId, cancellationToken);

        return member?.ProjectRole.ToString() ?? "None";
    }

    #endregion
}