using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Section.Commands.UploadSectionFile;

public record UploadSectionFileCommand(UploadSectionFileDto Dto, Guid Id) : ICommand<Guid>;

public class UploadSectionFileCommandValidator : AbstractValidator<UploadSectionFileCommand>
{
    public UploadSectionFileCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Id)
                    .NotEmpty()
                    .WithMessage(MessageCode.SectionIdIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.SectionIdIsRequired);
            });

        RuleFor(x => x.Dto.UploadFile)
            .NotNull()
            .WithMessage(MessageCode.SectionFileIsRequired);
    }
}

public class UploadSectionFileCommandHandler(IDocumentSession session, IMinIoCloudService minIo) : ICommandHandler<UploadSectionFileCommand, Guid>
{
    public async Task<Guid> Handle(UploadSectionFileCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        await session.BeginTransactionAsync(cancellationToken);

        var section = await session.LoadAsync<SectionEntity>(request.Id, cancellationToken)
                      ?? throw new NotFoundException(MessageCode.SectionIsNotExists, request.Id);

        await UploadFileAsync(dto.UploadFile, section, cancellationToken);

        await session.SaveChangesAsync(cancellationToken);

        return section.Id;
    }

    #region Methods

    private async Task UploadFileAsync(UploadFileBytes? file,
        SectionEntity entity,
        CancellationToken cancellationToken)
    {
        if (file == null) return;

        var name = $"{file.FileName}-{Guid.NewGuid()}-{entity.Id}";

        var result = await minIo.UploadFilesAsync(name, [file],
            AppConstants.Bucket.Sections,
            true,
            cancellationToken);

        var uploaded = result.FirstOrDefault();

        if (uploaded != null)
        {
            entity.UpdateFilePath(uploaded.PublicURL);
        }
    }

    #endregion
}