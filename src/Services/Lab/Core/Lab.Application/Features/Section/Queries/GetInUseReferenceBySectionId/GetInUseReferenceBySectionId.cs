using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Section.Queries.GetInUseReferenceBySectionId;

public record GetInUseReferenceBySectionIdQuery(Guid Id) : ICommand<GetInUseReferenceBySectionIdResult>;

public class GetInUseReferenceBySectionIdQueryValidator : AbstractValidator<GetInUseReferenceBySectionIdQuery>
{
    public GetInUseReferenceBySectionIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.SectionIdIsRequired)
            .NotNull()
            .WithMessage(MessageCode.SectionIdIsRequired);
    }
}

public class GetInUseReferenceBySectionIdQueryHandler(IDocumentSession session)
    : ICommandHandler<GetInUseReferenceBySectionIdQuery, GetInUseReferenceBySectionIdResult>
{
    public async Task<GetInUseReferenceBySectionIdResult> Handle(GetInUseReferenceBySectionIdQuery request, CancellationToken cancellationToken)
    {
        var section = await session.LoadAsync<SectionEntity>(request.Id, cancellationToken)
                      ?? throw new NotFoundException(MessageCode.SectionIsNotExists, request.Id.ToString());

        var inUseIds = (section.References ?? [])
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (inUseIds.Count == 0)
            return new GetInUseReferenceBySectionIdResult();

        var paper = await session.LoadAsync<PaperEntity>(section.PaperId, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.PaperIsNotExists, section.PaperId.ToString());

        var paperReferenceIds = (paper.References ?? [])
            .Where(x => inUseIds.Contains(x.PaperId))
            .Select(x => x.PaperId)
            .Distinct()
            .ToHashSet();

        var paperBankIds = inUseIds
            .Where(id => paperReferenceIds.Count == 0 || paperReferenceIds.Contains(id))
            .ToList();

        var paperBanks = await session.Query<PaperBankEntity>()
            .Where(x => paperBankIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var paperBankMap = paperBanks.ToDictionary(x => x.Id, ToPaperBankInfoDto);
        var items = paperBankIds
            .Where(paperBankMap.ContainsKey)
            .Select(id => paperBankMap[id])
            .ToList();

        var combinedReferenceContent = string.Join(
            $"{Environment.NewLine}{Environment.NewLine}",
            items
                .Select(x => x.ReferenceContent)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct());

        return new GetInUseReferenceBySectionIdResult
        {
            ReferenceContent = string.IsNullOrWhiteSpace(combinedReferenceContent)
                ? string.Empty
                : $"\\begin{{filecontents}}{{references.bib}}{Environment.NewLine}{Environment.NewLine}" +
                  combinedReferenceContent +
                  $"{Environment.NewLine}{Environment.NewLine}\\end{{filecontents}}{Environment.NewLine}{Environment.NewLine}" +
                  "\\addbibresource{references.bib}" +
                  $"{Environment.NewLine}{Environment.NewLine}\\printbibliography",
            PaperBanks = items
        };
    }

    private static PaperBankInfoDto ToPaperBankInfoDto(PaperBankEntity paperBank)
    {
        return new PaperBankInfoDto
        {
            Id = paperBank.Id,
            Title = paperBank.Title,
            Authors = paperBank.Authors,
            Publisher = paperBank.Publisher,
            Abstract = paperBank.Abstract,
            Doi = paperBank.Doi,
            FilePath = paperBank.FilePath,
            Status = paperBank.Status,
            ParsedText = paperBank.ParsedText,
            IsIngested = paperBank.IsIngested,
            IsAutoTagged = paperBank.IsAutoTagged,
            PublicationDate = paperBank.PublicationDate,
            PaperType = paperBank.PaperType,
            JournalName = paperBank.JournalName,
            Pages = paperBank.Pages,
            Number = paperBank.Number,
            Volume = paperBank.Volume,
            ConferenceName = paperBank.ConferenceName,
            ReferenceContent = paperBank.ReferenceContent,
            TagNames = paperBank.TagNames
        };
    }
}