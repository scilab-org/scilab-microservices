using JasperFx.Core;
using Lab.Application.Dtos.PaperAuthors;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.PaperAuthor.Commands.UpdatePaperAuthor;

public record UpdatePaperAuthorCommand(Guid Id, UpdatePaperAuthorDto Dto) : ICommand<Unit>;

public class UpdatePaperAuthorCommandValidator : AbstractValidator<UpdatePaperAuthorCommand>
{
    public UpdatePaperAuthorCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class UpdatePaperAuthorCommandHandler(IDocumentSession session,
    IManagementApiService managementApiService) : IRequestHandler<UpdatePaperAuthorCommand, Unit>
{
    public async Task<Unit> Handle(UpdatePaperAuthorCommand request, CancellationToken cancellationToken)
    {
        var role = await managementApiService.GetMyProjectRoleAsync(request.Dto.ProjectId, cancellationToken);
        if (role.IsNullOrEmpty() || !AuthorizeConstants.PaperAuthor.EqualsIgnoreCase(role))
            throw new UnauthorizedException(MessageCode.AccessDenied);
        
        var entity = await session.LoadAsync<PaperAuthorEntity>(request.Id, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.NotFound, request.Id.ToString());

        entity.Update(
            request.Dto.Name?.Trim(),
            request.Dto.OcrId?.Trim(),
            request.Dto.Email?.Trim(),
            request.Dto.PaperId,
            request.Dto.AuthorRoleId,
            request.Dto.MemberId);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
