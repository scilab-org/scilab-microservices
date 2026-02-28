using Lab.Application.Dtos.Papers;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
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
        var tagNames = NomalizeTagNames(dto.TagNames);

        await session.BeginTransactionAsync(cancellationToken);

        await EnsureTagsExistAsync(tagNames, cancellationToken);

        var entity = PaperEntity.Create(
            id: Guid.NewGuid(),
            title: dto.Title,
            abstractText: dto.Abstract,
            doi: dto.Doi,
            status: dto.Status ?? PaperStatus.Sampled,
            parsedText: dto.ParsedText,
            isIngested: dto.IsIngested,
            isAutoTagged: dto.IsAutoTagged,
            publicationDate: dto.PublicationDate,
            paperType: dto.PaperType,
            journalName: dto.JournalName,
            conferenceName: dto.ConferenceName,
            tagNames: tagNames);

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

    private async Task EnsureTagsExistAsync(
        List<string> tagNames,
        CancellationToken cancellationToken)
    {
        if (tagNames.Count == 0) return;

        var existingTags = await session
            .Query<TagEntity>()
            .Where(x => tagNames.Contains(x.Name))
            .ToListAsync(cancellationToken);

        var existingTagNames = existingTags
            .Select(x => x.Name)
            .ToHashSet();

        var newTagNames = tagNames
            .Where(x => !existingTagNames.Contains(x))
            .Distinct()
            .ToList();

        foreach (var name in newTagNames)
        {
            var tag = TagEntity.Create(Guid.NewGuid(), name);
            session.Store(tag);
        }
    }

    #endregion
}