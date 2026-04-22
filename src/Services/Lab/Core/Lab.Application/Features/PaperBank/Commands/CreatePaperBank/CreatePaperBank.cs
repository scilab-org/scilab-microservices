using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using EventSourcing.Events.Lab;
using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Repositories;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;
using MediatR;

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

                RuleFor(x => x.Dto.UploadPdfFile)
                    .NotNull()
                    .WithMessage(MessageCode.PaperFileIsRequired);

                RuleFor(x => x.Dto.ConferenceJournalId)
                    .NotNull()
                    .WithMessage(MessageCode.JournalIdIsRequired);
            });
    }
}

public class CreatePaperBankCommandHandler(
    IDocumentSession session,
    IMinIoCloudService minIo,
    IOutboxRepository outboxRepo)
    : ICommandHandler<CreatePaperBankCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(CreatePaperBankCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var keywords = NormalizeKeywords(dto.Keywords);

        await session.BeginTransactionAsync(cancellationToken);

        await EnsureTagsExistAsync(keywords, cancellationToken);

        var journal = await session.LoadAsync<ConferenceJournalEntity>(dto.ConferenceJournalId, cancellationToken)
                      ?? throw new NotFoundException(MessageCode.JournalIsNotExists, dto.ConferenceJournalId);

        var entity = PaperBankEntity.Create(
            id: Guid.NewGuid(),
            title: dto.Title,
            authors: dto.Authors,
            publisher: dto.Publisher,
            ranking: dto.Ranking,
            abstractText: dto.Abstract,
            doi: dto.Doi,
            url: dto.Url,
            code: dto.Code,
            parsedText: dto.ParsedText,
            isIngested: dto.IsIngested,
            isAutoTagged: dto.IsAutoTagged,
            publicationDate: dto.PublicationDate,
            paperType: dto.PaperType,
            pages: dto.Pages,
            number: dto.Number,
            volume: dto.Volume,
            conferenceJournalId: dto.ConferenceJournalId,
            referenceContent: dto.ReferenceContent,
            keywords: keywords,
            ingestStatus: IngestStatus.Pending);

        var (pdfFile, bibFile) = await UploadFilesAsync(dto, cancellationToken);
        entity.UpdateFilePath(pdfFile, bibFile);

        session.Store(entity);

        var message = new PaperIngestionEvent
        {
            PaperId = entity.Id,
            PaperName = entity.Title,
            ParsedText = entity.ParsedText ?? string.Empty,
            ReferenceKey = GenerateReferenceKey(entity),
            Authors = entity.Authors ?? string.Empty,
            Publisher = entity.Publisher ?? string.Empty,
            JournalName = journal.Name ?? string.Empty,
            Volume = entity.Volume ?? string.Empty,
            Pages = entity.Pages ?? string.Empty,
            Doi = entity.Doi ?? string.Empty,
            PublicationMonthYear = FormatMonthYear(entity.PublicationDate) ?? string.Empty,
        };

        var outbox = OutboxMessageEntity.Create(
            id: Guid.NewGuid(),
            eventType: message.EventType!,
            content: JsonSerializer.Serialize(message),
            occurredOnUtc: DateTimeOffset.UtcNow
        );

        await outboxRepo.AddMessageAsync(outbox, cancellationToken);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #endregion

    #region Methods

    private async Task<(string? PdfFile, string? BibFile)> UploadFilesAsync(
        CreatePaperBankDto dto,
        CancellationToken cancellationToken)
    {
        var pdfFile = await UploadFileAsync(dto.UploadPdfFile, cancellationToken);
        var bibFile = await UploadFileAsync(dto.UploadBibFile, cancellationToken);

        return (pdfFile, bibFile);
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
            AppConstants.Bucket.Papers,
            true,
            cancellationToken);

        return result.FirstOrDefault()?.PublicURL;
    }

    private static string GenerateReferenceKey(PaperBankEntity entity)
    {
        var year = entity.PublicationDate?.Year.ToString() ?? string.Empty;

        var authors = entity.Authors ?? string.Empty;
        var normalizedAuthors = Regex.Replace(authors, @"\s+and\s+", ", ", RegexOptions.IgnoreCase);

        var firstAuthorToken = normalizedAuthors
            .Split(',')
            .Select(p => p.Trim())
            .FirstOrDefault(p => !string.IsNullOrEmpty(p));

        var raw = firstAuthorToken
                  ?? (string.IsNullOrWhiteSpace(entity.Title) ? "Paper" : entity.Title);

        var authorToken = Regex.Replace(raw, @"[^A-Za-z0-9]+", string.Empty);

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

    private List<string> NormalizeKeywords(List<string>? keywords)
    {
        if (keywords == null) return new List<string>();
        return keywords.Select(x => x.Trim().ToLowerInvariant()).ToList();
    }

    private async Task EnsureTagsExistAsync(
        List<string> keywords,
        CancellationToken cancellationToken)
    {
        if (keywords.Count == 0) return;

        var existingTags = await session
            .Query<KeywordEntity>()
            .Where(x => keywords.Contains(x.Name))
            .ToListAsync(cancellationToken);

        var existingTagNames = existingTags
            .Select(x => x.Name)
            .ToHashSet();

        var newTagNames = keywords
            .Where(x => !existingTagNames.Contains(x))
            .Distinct()
            .ToList();

        foreach (var name in newTagNames)
        {
            var tag = KeywordEntity.Create(Guid.NewGuid(), name);
            session.Store(tag);
        }
    }

    #endregion
}