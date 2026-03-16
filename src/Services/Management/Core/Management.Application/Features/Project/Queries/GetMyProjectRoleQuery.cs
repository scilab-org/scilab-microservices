using Management.Application.Services;
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

public sealed class GetMyProjectRoleQueryHandler(
    IDocumentSession session,
    IRedisService redisService) : IQueryHandler<GetMyProjectRoleQuery, string>
{
    public const string ProjectRolesKey = "ProjectRoles:";
    public static string UserIdentifierKey(Guid userId) => $"{userId}:";
    
    #region Implementations

    public async Task<string> Handle(GetMyProjectRoleQuery req, CancellationToken cancellationToken)
    {
        var cacheKey = $"{ProjectRolesKey}{UserIdentifierKey(req.UserId)}{req.ProjectId}";

        var role = await redisService.GetOrSetCacheAsync(
            cacheKey,
            async ct =>
            {
                var member = await session.Query<MemberEntity>()
                    .FirstOrDefaultAsync(x => x.UserId == req.UserId && x.ProjectId == req.ProjectId, ct);

                return member?.ProjectRole.ToString() ?? "None";
            },
            TimeSpan.FromMinutes(10),
            cancellationToken);

        return role!;
    }

    #endregion
}