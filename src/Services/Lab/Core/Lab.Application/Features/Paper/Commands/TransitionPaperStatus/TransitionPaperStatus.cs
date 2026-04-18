using JasperFx.Core;
using Lab.Application.Dtos.Papers;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Lab.Domain.Services;
using Marten;

namespace Lab.Application.Features.Paper.Commands.TransitionPaperStatus;

public record TransitionPaperStatusCommand(
    Guid PaperId,
    TransitionPaperStatusDto Dto,
    Guid UserId,
    string UserName) : ICommand<Guid>;

public class TransitionPaperStatusCommandValidator : AbstractValidator<TransitionPaperStatusCommand>
{
    public TransitionPaperStatusCommandValidator()
    {
        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);

        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.ProjectId)
                    .NotEmpty()
                    .WithMessage(MessageCode.ProjectIdIsRequired);
            });
    }
}

public class TransitionPaperStatusCommandHandler(
    IDocumentSession session,
    IManagementApiService managementApiService) : ICommandHandler<TransitionPaperStatusCommand, Guid>
{
    #region Implementation

    public async Task<Guid> Handle(TransitionPaperStatusCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var paper = await session.LoadAsync<PaperEntity>(request.PaperId, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.PaperIsNotExists, request.PaperId.ToString());

        var latestHistory = await session.Query<PaperStatusHistoryEntity>()
            .Where(h => h.PaperId == request.PaperId)
            .OrderByDescending(h => h.CreatedOnUtc)
            .FirstOrDefaultAsync(cancellationToken);

        var currentStatus = latestHistory?.Status ?? SubmissionStatus.Draft;

        if (currentStatus == dto.TargetStatus)
            throw new ClientValidationException(
                MessageCode.DuplicateStatusTransition,
                dto.TargetStatus.ToString());

        if (!PaperStatusMachine.IsTransitionAllowed(currentStatus, dto.TargetStatus))
            throw new ClientValidationException(
                MessageCode.InvalidStatusTransition,
                $"{currentStatus} → {dto.TargetStatus}");

        // Validate PDF requirement for Submitted / Resubmitted
        if (PaperStatusMachine.RequiresPdf(dto.TargetStatus))
        {
            if (dto.PdfFileId is null || dto.PdfFileId == Guid.Empty)
                throw new ClientValidationException(
                    MessageCode.PdfFileIsRequired,
                    dto.TargetStatus.ToString());

            var pdfFile = await session.LoadAsync<PaperVersionFileEntity>(dto.PdfFileId.Value, cancellationToken)
                          ?? throw new NotFoundException(MessageCode.PdfFileNotFound, dto.PdfFileId.ToString()!);

            // Verify the PDF belongs to a version of this paper
            var paperVersion = await session.LoadAsync<PaperVersionEntity>(pdfFile.PaperVersionId, cancellationToken);
            if (paperVersion is null || paperVersion.PaperId != request.PaperId)
                throw new ClientValidationException(
                    MessageCode.PdfFileNotBelongToPaper,
                    dto.PdfFileId.ToString()!);
        }

        await AuthorizeAsync(request, dto, cancellationToken);

        var historyEntry = PaperStatusHistoryEntity.Create(
            paperId: request.PaperId,
            status: dto.TargetStatus,
            actorId: request.UserId,
            actorUserName: request.UserName,
            note: dto.Note,
            revisionType: dto.RevisionType,
            pdfFileId: PaperStatusMachine.RequiresPdf(dto.TargetStatus) ? dto.PdfFileId : null);

        await session.BeginTransactionAsync(cancellationToken);
        session.Store(historyEntry);
        await session.SaveChangesAsync(cancellationToken);

        return historyEntry.Id;
    }

    #endregion

    #region Methods

    private async Task AuthorizeAsync(
        TransitionPaperStatusCommand request,
        TransitionPaperStatusDto dto,
        CancellationToken cancellationToken)
    {
        if (PaperStatusMachine.RequiresAuthorRole(dto.TargetStatus))
        {
            var members = await managementApiService.GetSubProjectMembersByPaperIdAsync(
                request.PaperId, cancellationToken);

            var isAuthor = members.Any(m =>
                m.UserId == request.UserId &&
                m.Role.EqualsIgnoreCase(AuthorizeConstants.PaperAuthor));

            if (!isAuthor)
                throw new NoPermissionException(MessageCode.AccessDenied);
        }
        else if (PaperStatusMachine.RequiresEditorRole(dto.TargetStatus))
        {
            var projectRole = await managementApiService.GetMyProjectRoleAsync(
                dto.ProjectId, cancellationToken);

            var isEditor = !string.IsNullOrWhiteSpace(projectRole)
                           && (projectRole.EqualsIgnoreCase(AuthorizeConstants.ProjectManager)
                               || projectRole.EqualsIgnoreCase(AuthorizeConstants.ProjectAuthor));

            if (!isEditor)
                throw new NoPermissionException(MessageCode.AccessDenied);
        }
        else
        {
            throw new NoPermissionException(MessageCode.AccessDenied);
        }
    }

    #endregion
}
