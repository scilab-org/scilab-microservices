using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.PaperBank.Commands.UpdatePaperBank;

public record UpdatePaperBankCommand(Guid Id, UpdatePaperBankDto BankDto) : ICommand<Guid>;

public class UpdatePaperCommandVaBanklidator : AbstractValidator<UpdatePaperBankCommand>
{
    public UpdatePaperCommandVaBanklidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);

        RuleFor(x => x.BankDto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.BankDto)
                    .NotNull()
                    .WithMessage(MessageCode.BadRequest)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.BankDto.Title)
                            .NotEmpty()
                            .WithMessage(MessageCode.PaperTitleIsRequired)
                            .NotNull()
                            .WithMessage(MessageCode.PaperTitleIsRequired);

                        RuleFor(x => x.BankDto.PublicationDate)
                            .LessThanOrEqualTo(DateTimeOffset.UtcNow)
                            .When(x => x.BankDto.PublicationDate.HasValue)
                            .WithMessage(MessageCode.PaperPublicationDateInvalid);
                    });

                RuleFor(x => x.BankDto.PublicationDate)
                    .LessThanOrEqualTo(DateTimeOffset.UtcNow)
                    .When(x => x.BankDto.PublicationDate.HasValue)
                    .WithMessage(MessageCode.PaperPublicationDateInvalid);

                RuleFor(x => x.BankDto.ConferenceJournalId)
                    .NotEmpty()
                    .WithMessage(MessageCode.JournalIdIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.JournalIdIsRequired);
            });
    }
}

public class UpdatePaperCommandBankHandler(IDocumentSession session, IMinIoCloudService minIo)
    : IRequestHandler<UpdatePaperBankCommand, Guid>
{
    public async Task<Guid> Handle(UpdatePaperBankCommand request, CancellationToken cancellationToken)
    {
        var dto = request.BankDto;
        var keywords = NormalizeKeywords(dto.Keywords);

        await session.BeginTransactionAsync(cancellationToken);

        var entity = await session.LoadAsync<PaperBankEntity>(request.Id, cancellationToken)
                     ?? throw new ClientValidationException(MessageCode.PaperIsNotExists, request.Id);

        var journal = await session.LoadAsync<ConferenceJournalEntity>(dto.ConferenceJournalId, cancellationToken)
                      ?? throw new ClientValidationException(MessageCode.JournalIsNotExists, dto.ConferenceJournalId);

        var gapTypeIds = dto.GapTypeIds ?? [];
        foreach (var gapTypeId in gapTypeIds)
        {
            var exists = await session.LoadAsync<GapTypeEntity>(gapTypeId, cancellationToken);
            if (exists == null)
                throw new ClientValidationException(MessageCode.GapTypeIsNotExists, gapTypeId);
        }

        await EnsureKeywordsExistAsync(keywords, cancellationToken);

        entity.Update(
            title: dto.Title,
            authors: dto.Authors,
            publisher: dto.Publisher,
            ranking: dto.Ranking,
            abstractText: dto.Abstract,
            doi: dto.Doi,
            url: dto.Url,
            isIngested: dto.IsIngested,
            isAutoTagged: dto.IsAutoTagged,
            publicationDate: dto.PublicationDate,
            gapTypeIds: gapTypeIds,
            pages: dto.Pages,
            number: dto.Number,
            volume: dto.Volume,
            conferenceJournalId: journal.Id,
            referenceContent: dto.ReferenceContent,
            keywords: keywords);

        var bibFile = await UploadFilesAsync(dto, cancellationToken);
        entity.UpdateFilePath(bibUrl: bibFile);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #region Methods

    private async Task<string?> UploadFilesAsync(
        UpdatePaperBankDto dto,
        CancellationToken cancellationToken)
    {
        var bibFile = await UploadFileAsync(dto.UploadBibFile, cancellationToken);

        return bibFile;
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

    private List<string> NormalizeKeywords(List<string>? keywords)
    {
        if (keywords == null) return new List<string>();

        return keywords.Select(x => x.Trim().ToLowerInvariant()).ToList();
    }

    private async Task EnsureKeywordsExistAsync(
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