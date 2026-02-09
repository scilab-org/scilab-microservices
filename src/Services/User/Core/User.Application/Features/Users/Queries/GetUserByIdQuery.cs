#region using

using User.Application.Models.Results;
using User.Application.Services;

#endregion

namespace User.Application.Features.Users.Queries;

public sealed record GetUserByIdQuery(string UserId) : IQuery<GetUserByIdResult>;

public sealed class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    #region Ctors

    public GetUserByIdQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(MessageCode.UserIdIsRequired);
    }

    #endregion
}

public sealed class GetUserByIdQueryHandler(
    IKeycloakService keycloakService) : IQueryHandler<GetUserByIdQuery, GetUserByIdResult>
{
    #region Implementations

    public async Task<GetUserByIdResult> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await keycloakService.GetUserByIdAsync(
            userId: query.UserId,
            cancellationToken: cancellationToken);

        return new GetUserByIdResult(user);
    }

    #endregion
}
