using System.Globalization;
using System.Text.RegularExpressions;
using EventSourcing.Events.Lab;
using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Repositories;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using Newtonsoft.Json;

namespace Lab.Application.Features.PaperBank.Commands.CreatePaperBank;

public record CreatePaperBankCommand(CreatePaperBankDto Dto) : ICommand<Guid>;

public class CreatePaperBankCommandValidator : AbstractValidator<CreatePaperBankCommand>
{
    public CreatePaperBankCommandValidator()
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

public class CreatePaperBankCommandHandler(IDocumentSession session, IMinIoCloudService minIo, IOutboxRepository outboxRepo)
    : IRequestHandler<CreatePaperBankCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(CreatePaperBankCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var tagNames = NomalizeTagNames(dto.TagNames);

        await session.BeginTransactionAsync(cancellationToken);

        await EnsureTagsExistAsync(tagNames, cancellationToken);

        var entity = PaperBankEntity.Create(
            id: Guid.NewGuid(),
            title: dto.Title,
            authors: dto.Authors,
            publisher: dto.Publisher,
            abstractText: dto.Abstract,
            doi: dto.Doi,
            parsedText: dto.ParsedText,
            isIngested: dto.IsIngested,
            isAutoTagged: dto.IsAutoTagged,
            publicationDate: dto.PublicationDate,
            paperType: dto.PaperType,
            journalName: dto.JournalName,
            pages: dto.Pages,
            number: dto.Number,
            volume: dto.Volume,
            conferenceName: dto.ConferenceName,
            referenceContent: dto.ReferenceContent,
            tagNames: tagNames,
            ingestStatus: IngestStatus.Pending);

        await UploadFileAsync(dto.UploadFile, entity, cancellationToken);

        session.Store(entity);

        //publish event to outbox (for paper ingestion)
        var message = new PaperIngestionEvent
        {
            PaperId = entity.Id,
            PaperName = entity.Title,
            ParsedText = entity.ParsedText ?? string.Empty,
            ReferenceKey = GenerateReferenceKey(entity),
            Authors = entity.Authors,
            Publisher = entity.Publisher,
            JournalName = entity.JournalName,
            Volume = entity.Volume,
            Pages = entity.Pages,
            Doi = entity.Doi,
            PublicationMonthYear = FormatMonthYear(entity.PublicationDate),
        };

        var outbox = OutboxMessageEntity.Create(
            id: Guid.NewGuid(),
            eventType: message.EventType!,
            content: JsonConvert.SerializeObject(message),
            occurredOnUtc: DateTimeOffset.UtcNow
        );

        await outboxRepo.AddMessageAsync(outbox, cancellationToken);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #endregion

    #region Methods

    private async Task UploadFileAsync(UploadFileBytes? file,
        PaperBankEntity entity,
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

    private static string GenerateReferenceKey(PaperBankEntity entity)
    {
        var year = entity.PublicationDate?.Year.ToString() ?? string.Empty;

        // Normalise " and " separators (case-insensitive) → ", "
        var authors = entity.Authors ?? string.Empty;
        var normalizedAuthors = Regex.Replace(authors, @"\s+and\s+", ", ", RegexOptions.IgnoreCase);

        // First non-empty token after splitting on ","
        var firstAuthorToken = normalizedAuthors
            .Split(',')
            .Select(p => p.Trim())
            .FirstOrDefault(p => !string.IsNullOrEmpty(p));

        // Fallback chain: first author token → title → "Paper"
        var raw = firstAuthorToken
            ?? (string.IsNullOrWhiteSpace(entity.Title) ? "Paper" : entity.Title);

        // Strip non-alphanumeric characters
        var authorToken = Regex.Replace(raw, @"[^A-Za-z0-9]+", string.Empty);

        // Prefix "Paper" if the token starts with a digit
        if (authorToken.Length > 0 && char.IsDigit(authorToken[0]))
            authorToken = "Paper" + authorToken;

        if (string.IsNullOrEmpty(authorToken))
            authorToken = "Paper";

        return $"{authorToken}{year.Trim()}";
    }

    private static string? FormatMonthYear(DateTimeOffset? date)
        => date.HasValue
            ? date.Value.ToString("MMMM yyyy", CultureInfo.InvariantCulture)
            : null;

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