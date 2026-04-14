using Lab.Application.Dtos.Journals;
using Lab.Application.Services;
using Common.Constants;
using JasperFx.Core;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Journal.Commands.UpdateJournal;

public record UpdateJournalCommand(UpdateJournalEntityDto Dto, Guid ProjectId, string UserName) : ICommand<Guid>;

public class UpdateJournalCommandValidator : AbstractValidator<UpdateJournalCommand>
{
    public UpdateJournalCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.Id)
                    .NotEmpty().WithMessage(MessageCode.JournalIdIsRequired);

                RuleFor(x => x.ProjectId)
                    .NotEmpty().WithMessage(MessageCode.JournalProjectIdIsRequired);

                RuleFor(x => x.Dto.TexUploadFile)
                    .Must(file => file == null || file.FileName.EndsWith(".tex", StringComparison.OrdinalIgnoreCase))
                    .WithMessage(MessageCode.JournalTexFileInvalidExtension);

                RuleFor(x => x.Dto.PdfUploadFile)
                    .Must(file => file == null || file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    .WithMessage(MessageCode.JournalPdfFileInvalidExtension);
            });
    }
}

public class UpdateJournalCommandHandler(IDocumentSession session, IMinIoCloudService minIo, IManagementApiService managementApiService) : IRequestHandler<UpdateJournalCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(UpdateJournalCommand request, CancellationToken cancellationToken)
    {
        var role = await managementApiService.GetMyProjectRoleAsync(request.ProjectId, cancellationToken);
        if (string.IsNullOrEmpty(role) && !AuthorizeConstants.ProjectManager.EqualsIgnoreCase(role!))
        {
            throw new UnauthorizedException(MessageCode.Unauthorized);
        }

        await session.BeginTransactionAsync(cancellationToken);

        var entity = await session.LoadAsync<ConferenceJournalEntity>(request.Dto.Id, cancellationToken);

        if (entity == null)
            throw new ClientValidationException(MessageCode.JournalIsNotExists, request.Dto.Id);

        if (entity.ProjectId != request.ProjectId)
            throw new ClientValidationException(MessageCode.JournalIsNotExists, request.Dto.Id);

        var normalizedName = "";
        if (request.Dto.Name != null)
            normalizedName = request.Dto.Name.Trim();

        if (!string.IsNullOrEmpty(normalizedName) && normalizedName != entity.Name)
        {
            var existingJournalName = await session.Query<ConferenceJournalEntity>()
                .FirstOrDefaultAsync(x => x.Name == normalizedName && x.ProjectId == request.ProjectId && x.Id != request.Dto.Id, cancellationToken);

            if (existingJournalName != null)
                throw new ClientValidationException(MessageCode.JournalNameAlreadyExists, normalizedName);
        }

        entity.Update(
            name: normalizedName != "" ? normalizedName : null,
            startAt: request.Dto.StartAt,
            endAt: request.Dto.EndAt,
            style: request.Dto.Style,
            texFile: null,
            pdfFile: null,
            lastModifiedBy: request.UserName);

        var (texFile, pdfFile) = await UploadFilesAsync(request.Dto, cancellationToken);
        entity.UpdateFilePath(texFile, pdfFile);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    private async Task<(string? TexFile, string? PdfFile)> UploadFilesAsync(
        UpdateJournalEntityDto dto,
        CancellationToken cancellationToken)
    {
        var texFile = await UploadFileAsync(dto.TexUploadFile, cancellationToken);
        var pdfFile = await UploadFileAsync(dto.PdfUploadFile, cancellationToken);

        return (texFile, pdfFile);
    }

    private async Task<string?> UploadFileAsync(UploadFileBytes? file, CancellationToken cancellationToken)
    {
        if (file == null) return null;

        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
        var extension = Path.GetExtension(file.FileName);
        var shortId = Guid.NewGuid().ToString("N")[..8];
        var name = $"{fileNameWithoutExtension}-{shortId}{extension}";

        var result = await minIo.UploadFilesAsync(
            name,
            [file],
            AppConstants.Bucket.ConferenceJournals,
            true,
            cancellationToken);

        return result.FirstOrDefault()?.PublicURL;
    }

    #endregion
}