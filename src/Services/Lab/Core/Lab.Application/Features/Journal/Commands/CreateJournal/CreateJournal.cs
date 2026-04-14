using JasperFx.Core;
using Lab.Application.Dtos.Journals;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Journal.Commands.CreateJournal;

public record CreateJournalCommand(CreateJournalEntityDto Dto, Guid ProjectId, string UserName) : ICommand<Guid>;

public class CreateJournalCommandValidator : AbstractValidator<CreateJournalCommand>
{
    public CreateJournalCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.Name)
                    .NotEmpty().WithMessage(MessageCode.JournalNameIsRequired)
                    .NotNull().WithMessage(MessageCode.JournalNameIsRequired);

                RuleFor(x => x.ProjectId)
                    .NotEmpty().WithMessage(MessageCode.JournalProjectIdIsRequired);

                RuleFor(x => x.Dto.StartAt)
                    .NotEmpty().WithMessage(MessageCode.JournalStartDateIsRequired)
                    .LessThan(x => x.Dto.EndAt).WithMessage(MessageCode.JournalStartDateMustBeforeEndDate);

                RuleFor(x => x.Dto.EndAt)
                    .NotEmpty().WithMessage(MessageCode.JournalEndDateIsRequired);

                RuleFor(x => x.Dto.Sections)
                    .NotNull().WithMessage(MessageCode.JournalSectionsAreRequired)
                    .NotEmpty().WithMessage(MessageCode.JournalSectionsAreRequired);

                RuleFor(x => x.Dto.TexUploadFile)
                    .Must(file => file == null || file.FileName.EndsWith(".tex", StringComparison.OrdinalIgnoreCase))
                    .WithMessage(MessageCode.JournalTexFileInvalidExtension);

                RuleFor(x => x.Dto.PdfUploadFile)
                    .Must(file => file == null || file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    .WithMessage(MessageCode.JournalPdfFileInvalidExtension);
            });
    }
}

public class CreateJournalCommandHandler(IDocumentSession session, IMinIoCloudService minIo, IManagementApiService managementApiService) : IRequestHandler<CreateJournalCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(CreateJournalCommand request, CancellationToken cancellationToken)
    {
        var role = await managementApiService.GetMyProjectRoleAsync(request.ProjectId, cancellationToken);
        if (string.IsNullOrEmpty(role) && !AuthorizeConstants.ProjectManager.EqualsIgnoreCase(role!))
        {
            throw new UnauthorizedException(MessageCode.Unauthorized);
        }

        await session.BeginTransactionAsync(cancellationToken);
        var id = Guid.NewGuid();

        var normalizedName = request.Dto.Name.Trim();

        var existingJournal = await session.Query<ConferenceJournalEntity>()
            .FirstOrDefaultAsync(x => x.Name == normalizedName &&
                                      x.ProjectId == request.ProjectId, cancellationToken);

        if (existingJournal != null)
            throw new ClientValidationException(MessageCode.JournalNameAlreadyExists, request.Dto.Name);

        var entity = ConferenceJournalEntity.Create(
            id: id,
            name: normalizedName,
            projectId: request.ProjectId,
            startAt: request.Dto.StartAt,
            endAt: request.Dto.EndAt,
            style: request.Dto.Style,
            texFile: null,
            pdfFile: null,
            createdBy: request.UserName);

        var (texFile, pdfFile) = await UploadFilesAsync(request.Dto, cancellationToken);
        entity.UpdateFilePath(texFile, pdfFile);

        session.Store(entity);

        var template = TemplateEntity.Create(
            code: request.Dto.TemplateCode ?? $"TEMPLATE-{id:N}",
            description: request.Dto.TemplateDescription ?? $"Default template for {normalizedName}",
            conferenceJournalId: id,
            sections: request.Dto.Sections,
            createdBy: request.UserName);

        session.Store(template);

        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    private async Task<(string? TexFile, string? PdfFile)> UploadFilesAsync(
        CreateJournalEntityDto dto,
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