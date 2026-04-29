using Lab.Application.Dtos.Journals;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Journal.Commands.UpdateJournal;

public record UpdateJournalCommand(UpdateJournalEntityDto Dto, Guid Id, string UserName) : ICommand<Guid>;

public class UpdateJournalCommandValidator : AbstractValidator<UpdateJournalCommand>
{
    public UpdateJournalCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Id)
                    .NotEmpty().WithMessage(MessageCode.JournalIdIsRequired);

                RuleFor(x => x.Dto.Name)
                    .NotEmpty().WithMessage(MessageCode.JournalNameIsRequired)
                    .NotNull().WithMessage(MessageCode.JournalNameIsRequired);

                RuleFor(x => x.Dto.Ranking)
                    .NotEmpty().WithMessage(MessageCode.JournalRankingIsRequired)
                    .NotNull().WithMessage(MessageCode.JournalRankingIsRequired);

                RuleFor(x => x.Dto.Url)
                    .NotEmpty().WithMessage(MessageCode.JournalUrlIsRequired)
                    .NotNull().WithMessage(MessageCode.JournalUrlIsRequired);

                RuleFor(x => x.Dto.ISSN)
                    .NotEmpty().WithMessage(MessageCode.JournalIssnIsRequired)
                    .NotNull().WithMessage(MessageCode.JournalIssnIsRequired);

                RuleFor(x => x.Dto.TemplateIds)
                    .NotNull().WithMessage(MessageCode.TemplateIdIsRequired)
                    .Must(ids => ids is { Count: > 0 }).WithMessage(MessageCode.TemplateIdIsRequired);

                RuleFor(x => x.Dto.TexUploadFile)
                    .Must(file => file == null || file.FileName.EndsWith(".tex", StringComparison.OrdinalIgnoreCase))
                    .WithMessage(MessageCode.JournalTexFileInvalidExtension);

                RuleFor(x => x.Dto.PdfUploadFile)
                    .Must(file => file == null || file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    .WithMessage(MessageCode.JournalPdfFileInvalidExtension);
            });
    }
}

public class UpdateJournalCommandHandler(
    IDocumentSession session,
    IMinIoCloudService minIo) : IRequestHandler<UpdateJournalCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(UpdateJournalCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var dto = request.Dto;

        var entity = await session.LoadAsync<ConferenceJournalEntity>(request.Id, cancellationToken)
                     ?? throw new ClientValidationException(MessageCode.JournalIsNotExists, request.Id);

        var templates = await session.Query<TemplateEntity>()
            .Where(x => request.Dto.TemplateIds.Contains(x.Id))
            .Distinct()
            .ToListAsync(cancellationToken);

        entity.Update(
            name: dto.Name,
            ranking: dto.Ranking,
            url: dto.Url,
            style: dto.Style,
            issn: dto.ISSN,
            templateIds: templates.Select(x => x.Id).ToList(),
            lastModifiedBy: request.UserName);

        var (texFile, pdfFile) = await UploadFilesAsync(request.Dto, cancellationToken);
        entity.UpdateFilePath(texFile, pdfFile);

        session.Update(entity);
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