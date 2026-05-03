using Common.Constants;
using Common.Models;
using Lab.Application.Dtos.Papers;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Paper.Commands.UploadPaperFile;

public record UploadPaperFileCommand(
    Guid PaperId,
    CreatePaperVersionFileDto Dto,
    string UserName) : ICommand<Guid>;

public class UploadPaperFileCommandValidator : AbstractValidator<UploadPaperFileCommand>
{
    public UploadPaperFileCommandValidator()
    {
        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);

        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.UploadFile)
                    .NotNull()
                    .WithMessage(MessageCode.PdfFileIsRequired);
            });
    }
}

public class UploadPaperFileCommandHandler(IDocumentSession session, IMinIoCloudService minIo)
    : ICommandHandler<UploadPaperFileCommand, Guid>
{
    public async Task<Guid> Handle(UploadPaperFileCommand request, CancellationToken cancellationToken)
    {
        var paper = await session.LoadAsync<PaperEntity>(request.PaperId, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.PaperIsNotExists, request.PaperId.ToString());

        var uploadFile = request.Dto.UploadFile;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(uploadFile.FileName);
        var shortId = Guid.NewGuid().ToString("N")[..8];
        var name = $"{fileNameWithoutExtension}-{shortId}";

        var result = await minIo.UploadFilesAsync(name, [uploadFile],
            AppConstants.Bucket.Papers,
            true,
            cancellationToken);

        var uploaded = result.FirstOrDefault()
                       ?? throw new ClientValidationException(MessageCode.PdfFileIsRequired, "Upload failed");

        var entity = PaperVersionFileEntity.Create(
            paperVersionId: null,
            fileName: uploadFile.FileName,
            fileUrl: uploaded.PublicURL!,
            note: request.Dto.Note,
            createdBy: request.UserName);

        await session.BeginTransactionAsync(cancellationToken);
        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
