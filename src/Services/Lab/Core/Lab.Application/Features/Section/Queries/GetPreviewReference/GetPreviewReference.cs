using Lab.Application.Dtos.Sections;
using Lab.Application.Models.Results;
using Lab.Application.Dtos.PaperBanks;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Section.Queries.GetPreviewReference;

public record GetPreviewReferenceQuery(PreviewReferenceDto Dto) : ICommand<GetInUseReferenceBySectionIdResult>;

public class GetPreviewReferenceQueryValidator : AbstractValidator<GetPreviewReferenceQuery>
{
    public GetPreviewReferenceQueryValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.PaperBankIds)
                    .NotNull()
                    .Must(ids => ids != null && ids.All(id => id != Guid.Empty))
                    .WithMessage(MessageCode.PaperBankIdsIsRequired);
            });
    }
}

public class
    GetPreviewReferenceQueryHandler(IDocumentSession session) : ICommandHandler<GetPreviewReferenceQuery, GetInUseReferenceBySectionIdResult>
{
    public async Task<GetInUseReferenceBySectionIdResult> Handle(GetPreviewReferenceQuery request,
        CancellationToken cancellationToken)
    {
        var requestedIds = request.Dto.PaperBankIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (requestedIds.Count == 0)
            return new GetInUseReferenceBySectionIdResult();

        var paperBanks = await session.Query<PaperBankEntity>()
            .Where(x => requestedIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var paperBankMap = paperBanks.ToDictionary(x => x.Id, ToPaperBankInfoDto);
        var items = requestedIds
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
                : $"\\begin{{filecontents*}}{{references.bib}}{Environment.NewLine}{Environment.NewLine}" +
                  combinedReferenceContent +
                  $"{Environment.NewLine}{Environment.NewLine}\\end{{filecontents*}}{Environment.NewLine}{Environment.NewLine}" +
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