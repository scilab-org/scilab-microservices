using Lab.Application.Dtos.PaperBanks;
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
            });
    }
}

public class UpdatePaperCommandBankHandler(IDocumentSession session)
    : IRequestHandler<UpdatePaperBankCommand, Guid>
{
    public async Task<Guid> Handle(UpdatePaperBankCommand request, CancellationToken cancellationToken)
    {
        var dto = request.BankDto;
        var tagNames = NomalizeTagNames(dto.TagNames);

        await session.BeginTransactionAsync(cancellationToken);

        var entity = await session.LoadAsync<PaperBankEntity>(request.Id, cancellationToken)
                     ?? throw new ClientValidationException(MessageCode.PaperIsNotExists, request.Id);

        await EnsureTagsExistAsync(tagNames, cancellationToken);

        entity.Update(
            title: dto.Title,
            abstractText: dto.Abstract,
            doi: dto.Doi,
            status: dto.Status,
            isIngested: dto.IsIngested,
            isAutoTagged: dto.IsAutoTagged,
            publicationDate: dto.PublicationDate,
            paperType: dto.PaperType,
            journalName: dto.JournalName,
            conferenceName: dto.ConferenceName,
            tagNames: tagNames,
            ingestStatus: dto.IngestStatus);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #region Methods

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