using JasperFx.Core;
using Lab.Application.Dtos.Journals;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Journal.Commands.CreateJournal;

public record CreateJournalCommand(CreateJournalEntityDto Dto, string UserName) : ICommand<Guid>;

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

                RuleFor(x => x.Dto.Ranking)
                    .NotEmpty().WithMessage(MessageCode.JournalRankingIsRequired)
                    .NotNull().WithMessage(MessageCode.JournalRankingIsRequired);

                RuleFor(x => x.Dto.Url)
                    .NotEmpty().WithMessage(MessageCode.JournalUrlIsRequired)
                    .NotNull().WithMessage(MessageCode.JournalUrlIsRequired);

                RuleFor(x => x.Dto.TemplateId)
                    .NotEmpty().WithMessage(MessageCode.TemplateIdIsRequired)
                    .NotNull().WithMessage(MessageCode.TemplateIdIsRequired);

                RuleFor(x => x.Dto.TexUploadFile)
                    .Must(file => file == null || file.FileName.EndsWith(".tex", StringComparison.OrdinalIgnoreCase))
                    .WithMessage(MessageCode.JournalTexFileInvalidExtension);

                RuleFor(x => x.Dto.PdfUploadFile)
                    .Must(file => file == null || file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    .WithMessage(MessageCode.JournalPdfFileInvalidExtension);
            });
    }
}

public class CreateJournalCommandHandler(
    IDocumentSession session,
    IMinIoCloudService minIo) : IRequestHandler<CreateJournalCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(CreateJournalCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var normalizedName = request.Dto.Name.Trim();

        var existingJournal = await session.Query<ConferenceJournalEntity>()
            .FirstOrDefaultAsync(x => x.Name == normalizedName, cancellationToken);

        if (existingJournal != null)
            throw new ClientValidationException(MessageCode.JournalNameAlreadyExists, request.Dto.Name);

        var template = await session.LoadAsync<TemplateEntity>(request.Dto.TemplateId, cancellationToken)
                       ?? throw new NotFoundException(MessageCode.TemplateIsNotExists, request.Dto.TemplateId);

        var entity = ConferenceJournalEntity.Create(
            id: Guid.NewGuid(),
            name: normalizedName,
            ranking: request.Dto.Ranking,
            url: request.Dto.Url,
            style: request.Dto.Style,
            templateId: template.Id,
            texFile: null,
            pdfFile: null,
            createdBy: request.UserName);

        var (texFile, pdfFile) = await UploadFilesAsync(request.Dto, cancellationToken);
        entity.UpdateFilePath(texFile, pdfFile);

        session.Store(entity);

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
        var shortId = Guid.NewGuid().ToString("N")[..8];
        var name = $"{fileNameWithoutExtension}-{shortId}";

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