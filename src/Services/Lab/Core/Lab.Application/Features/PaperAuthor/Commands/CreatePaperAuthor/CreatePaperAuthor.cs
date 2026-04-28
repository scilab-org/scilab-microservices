using JasperFx.Core;
using Lab.Application.Dtos.PaperAuthors;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.PaperAuthor.Commands.CreatePaperAuthor;

public record CreatePaperAuthorCommand(CreatePaperAuthorDto Dto) : ICommand<Guid>;

public class CreatePaperAuthorCommandValidator : AbstractValidator<CreatePaperAuthorCommand>
{
    public CreatePaperAuthorCommandValidator()
    {
        RuleFor(x => x.Dto.Name).NotEmpty();
        RuleFor(x => x.Dto.Email).NotEmpty();
        RuleFor(x => x.Dto.PaperId).NotEmpty();
        RuleFor(x => x.Dto.AuthorRoleId).NotEmpty();
        RuleFor(x => x.Dto.MemberId).NotEmpty();
    }
}

public class CreatePaperAuthorCommandHandler(
    IDocumentSession session,
    IManagementApiService managementApiService) : IRequestHandler<CreatePaperAuthorCommand, Guid>
{
    public async Task<Guid> Handle(CreatePaperAuthorCommand request, CancellationToken cancellationToken)
    {
        
        // var role = await managementApiService.GetMyProjectRoleAsync(request.Dto.ProjectId, cancellationToken);
        // if (role.IsNullOrEmpty() || !AuthorizeConstants.PaperAuthor.EqualsIgnoreCase(role))
        //     throw new UnauthorizedException(MessageCode.AccessDenied);
        
        var entity = PaperAuthorEntity.Create(
            Guid.NewGuid(),
            request.Dto.Name.Trim(),
            request.Dto.OcrId?.Trim(),
            request.Dto.Email.Trim(),
            request.Dto.PaperId,
            request.Dto.AuthorRoleId,
            request.Dto.MemberId,
            request.Dto.AffiliationId,
            request.Dto.AffiliationName);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
