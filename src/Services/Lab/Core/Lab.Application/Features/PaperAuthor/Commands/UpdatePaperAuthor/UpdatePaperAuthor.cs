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
        if (role.IsNullOrEmpty() || !AuthorizeConstants.ProjectManager.EqualsIgnoreCase(role))
            throw new UnauthorizedException(MessageCode.AccessDenied);
        
        var entity = await session.LoadAsync<PaperAuthorEntity>(request.Id, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.NotFound, request.Id.ToString());

        entity.Update(
            name: request.Dto.Name?.Trim(),
            ocrId: request.Dto.OcrId?.Trim(),
            email: request.Dto.Email?.Trim(),
            paperId: request.Dto.PaperId,
            authorRoleId: request.Dto.AuthorRoleId,
            memberId: request.Dto.MemberId,
            affiliationId: request.Dto.AffiliationId,
            affiliationName: request.Dto.AffiliationName?.Trim());

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
