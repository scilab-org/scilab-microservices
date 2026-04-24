#region using

using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Swagger.Extensions;
using Common.Constants;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using User.Api.Constants;
using User.Api.Models;
using User.Application.Dtos.Users;
using User.Application.Features.Users;

#endregion

namespace User.Api.Endpoints;

public sealed class CreateUser : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Users.Create, HandleCreateUserAsync)
            .WithTags(ApiRoutes.Users.Tags)
            .WithName(nameof(CreateUser))
            .WithMultipartForm<CreateUserRequest>()
            .Produces<ApiCreatedResponse<string>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiCreatedResponse<string>> HandleCreateUserAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromForm] CreateUserRequest req)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (!currentUser.HasGroups(AuthorizeConstants.SystemAdmin))
        {
            throw new UnauthorizedAccessException();
        }
        
        var dto = new CreateUserDto
        {
            Username = req.Username,
            Email = req.Email,
            FirstName = req.FirstName,
            LastName = req.LastName,
            OcrId = req.OcrId,
            InitialPassword = req.InitialPassword,
            TemporaryPassword = req.TemporaryPassword,
            GroupNames = ["app:user"]
        };

        if (req.AvatarImage is { Length: > 0 })
        {
            using var ms = new MemoryStream();
            await req.AvatarImage.CopyToAsync(ms);
            dto.AvatarImage = new UploadFileBytes
            {
                FileName = req.AvatarImage.FileName,
                ContentType = req.AvatarImage.ContentType,
                Bytes = ms.ToArray()
            };
        }

        var command = new CreateUserCommand(dto, Actor.User(currentUser.Email));
        var result = await sender.Send(command);

        return new ApiCreatedResponse<string>(result);
    }

    #endregion
}
