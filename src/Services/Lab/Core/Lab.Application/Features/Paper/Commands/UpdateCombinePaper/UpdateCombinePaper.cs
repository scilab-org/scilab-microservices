using JasperFx.Core;
using Lab.Application.Dtos.Papers;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Paper.Commands.UpdateCombinePaper;

public sealed record UpdateCombinePaperCommand(Guid PaperId, Guid VersionId, string UserName, UpdateCombinePaperDto Dto) : ICommand<Guid>;

public class UpdateCombinePaperCommandValidator : AbstractValidator<UpdateCombinePaperCommand>
{
    public UpdateCombinePaperCommandValidator()
    {
        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired)
            .NotNull()
            .WithMessage(MessageCode.PaperIdIsRequired);
        RuleFor(x => x.VersionId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperCombineVersionIdIsRequired)
            .NotNull()
            .WithMessage(MessageCode.PaperCombineVersionIdIsRequired);
        RuleFor(x => x.Dto.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.ProjectIdIsRequired)
            .NotNull()
            .WithMessage(MessageCode.ProjectIdIsRequired);
         RuleFor(x => x.Dto.Content)
            .NotEmpty()
            .WithMessage(MessageCode.PaperCombineContentIsRequired)
            .NotNull()
            .WithMessage(MessageCode.PaperCombineContentIsRequired);
    }
}

public class UpdateCombinePaperCommandHandler(IDocumentSession session, IManagementApiService managementApiService) : ICommandHandler<UpdateCombinePaperCommand, Guid>
{
    public async Task<Guid> Handle(UpdateCombinePaperCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var role = await managementApiService.GetMyProjectRoleAsync(dto.ProjectId, cancellationToken);
        if (role.IsNullOrEmpty() || !AuthorizeConstants.PaperAuthor.EqualsIgnoreCase(role))
            throw new UnauthorizedException(MessageCode.AccessDenied);

        await session.BeginTransactionAsync(cancellationToken);

        var paper = await session.LoadAsync<PaperEntity>(request.PaperId, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.PaperIsNotExists, request.PaperId.ToString());

        var version = paper.Versions.FirstOrDefault(x => x.Id == request.VersionId)
                      ?? throw new NotFoundException(MessageCode.PaperCombineIsNotExists, request.VersionId.ToString());

        version.Content = dto.Content;
        version.LastModifiedBy = request.UserName;

        paper.UpdatePaperVersion(
            versionId: version.Id,
            content: version.Content,
            lastModifiedBy: version.LastModifiedBy);

        session.Update(paper);
        await session.SaveChangesAsync(cancellationToken);

        return version.Id;
    }
}