using Lab.Application.Dtos.Papers;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Paper.Commands.CreatePaper;

public record CreatePaperCommand(CreatePaperDto Dto) : ICommand<Guid>;

public class CreatePaperCommandValidator : AbstractValidator<CreatePaperCommand>
{
    public CreatePaperCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.Title)
                    .NotEmpty()
                    .WithMessage(MessageCode.PaperTitleIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.PaperTitleIsRequired);

                RuleFor(x => x.Dto.PublicationDate)
                    .LessThanOrEqualTo(DateTimeOffset.UtcNow)
                    .When(x => x.Dto.PublicationDate.HasValue)
                    .WithMessage(MessageCode.PaperPublicationDateInvalid);

                RuleFor(x => x.Dto.UploadFile)
                    .NotNull()
                    .WithMessage(MessageCode.PaperFileIsRequired);
            });
    }
}

public class CreatePaperCommandHandler(IDocumentSession session, IMinIoCloudService minIo)
    : IRequestHandler<CreatePaperCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(CreatePaperCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        await session.BeginTransactionAsync(cancellationToken);

        var entity = PaperEntity.Create(
            id: Guid.NewGuid(),
            title: dto.Title,
            abstractText: dto.Abstract,
            doi: dto.Doi,
            status: dto.Status,
            parsedText: dto.ParsedText,
            isIngested: dto.IsIngested,
            isAutoTagged: dto.IsAutoTagged,
            publicationDate: dto.PublicationDate,
            paperType: dto.PaperType,
            journalName: dto.JournalName,
            conferenceName: dto.ConferenceName,
            tagNames: NomalizeTagNames(dto.TagNames));

        await UploadFileAsync(dto.UploadFile, entity, cancellationToken);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #endregion

    #region Methods

    private async Task UploadFileAsync(UploadFileBytes? file,
        PaperEntity entity,
        CancellationToken cancellationToken)
    {
        if (file == null) return;

        var result = await minIo.UploadFilesAsync(entity.Id.ToString(), [file],
            AppConstants.Bucket.Papers,
            true,
            cancellationToken);

        var uploaded = result.FirstOrDefault();

        if (uploaded != null)
        {
            entity.UpdateFilePath(uploaded.PublicURL);
        }
    }

    private List<string> NomalizeTagNames(List<string>? tagNames)
    {
        if (tagNames == null) return new List<string>();

        return tagNames.Select(x => x.Trim().ToLowerInvariant()).ToList();
    }

    #endregion
}