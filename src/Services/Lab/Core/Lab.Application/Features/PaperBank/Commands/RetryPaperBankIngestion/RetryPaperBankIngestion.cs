using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using EventSourcing.Events.Lab;
using Lab.Application.Repositories;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;
using MediatR;

namespace Lab.Application.Features.PaperBank.Commands.RetryPaperBankIngestion;

public record RetryPaperBankIngestionCommand(Guid PaperId) : ICommand<Guid>;

public class RetryPaperBankIngestionValidator : AbstractValidator<RetryPaperBankIngestionCommand>
{
    public RetryPaperBankIngestionValidator()
    {
        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);
    }
}

public class RetryPaperBankIngestionHandler(
    IDocumentSession session,
    IOutboxRepository outboxRepo)
    : ICommandHandler<RetryPaperBankIngestionCommand, Guid>
{
    public async Task<Guid> Handle(RetryPaperBankIngestionCommand request, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<PaperBankEntity>(request.PaperId, cancellationToken)
                     ?? throw new NotFoundException(MessageCode.PaperIsNotExists, request.PaperId);

        if (entity.IngestStatus != IngestStatus.Failed)
            throw new ClientValidationException(MessageCode.BadRequest, request.PaperId);

        var journal = entity.ConferenceJournalId.HasValue
            ? await session.LoadAsync<ConferenceJournalEntity>(entity.ConferenceJournalId.Value, cancellationToken)
            : null;

        entity.UpdateIngestionStatus(isIngested: false, ingestStatus: IngestStatus.Pending);
        session.Store(entity);

        var message = new PaperIngestionEvent
        {
            PaperId = entity.Id,
            PaperName = entity.Title,
            ParsedText = entity.ParsedText ?? string.Empty,
            ReferenceKey = GenerateReferenceKey(entity),
            Authors = entity.Authors ?? string.Empty,
            Publisher = entity.Publisher ?? string.Empty,
            JournalName = journal?.Name ?? string.Empty,
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
}
