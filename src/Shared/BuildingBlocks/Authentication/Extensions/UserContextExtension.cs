#region using

using Common.Constants;
using Common.Models.Context;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

#endregion

namespace BuildingBlocks.Authentication.Extensions;

public static class UserContextExtension
{
    #region Methods

    public static UserContext GetCurrentUser(this IHttpContextAccessor context)
    {
        var identity = context.HttpContext?.User;
        var userId = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        var userName = identity?.FindFirst(CustomClaimTypes.UserName)?.Value ?? string.Empty;
        var firstName = identity?.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty;
        var lastName = identity?.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty;
        var email = identity?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        var groups = identity?.FindAll(CustomClaimTypes.Groups).Select(c => c.Value).ToList() ?? [];
        var roles = identity?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? [];
        bool.TryParse(identity?.FindFirst(CustomClaimTypes.EmailVerified)?.Value, out bool emailVerified);

        return new UserContext()
        {
            EmailVerified = emailVerified,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Id = userId,
            UserName = userName,
            Groups = groups,
            Roles = roles
        };
    }

    #endregion
}
