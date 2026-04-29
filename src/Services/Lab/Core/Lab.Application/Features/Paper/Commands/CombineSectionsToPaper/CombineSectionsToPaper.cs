using JasperFx.Core;
using Lab.Application.Dtos.Papers;
using Lab.Application.Models.Results;
using Lab.Application.Services;
using Lab.Domain.Constants;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;

namespace Lab.Application.Features.Paper.Commands.CombineSectionsToPaper;

public record CombineSectionsToPaperCommand(Guid PaperId, CreatePaperCombineDto Dto, string UserName)
    : ICommand<CombineSectionsToPaperResult>;

public class CombineSectionsToPaperCommandValidator : AbstractValidator<CombineSectionsToPaperCommand>
{
    public CombineSectionsToPaperCommandValidator()
    {
        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired)
            .NotNull()
            .WithMessage(MessageCode.PaperIdIsRequired);
        RuleFor(x => x.Dto.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.ProjectIdIsRequired)
            .NotNull()
            .WithMessage(MessageCode.ProjectIdIsRequired);
    }
}

public class CombineSectionsToPaperCommandHandler(
    IDocumentSession session,
    IManagementApiService managementApiService,
    IAiApiService aiApiService,
    IHttpClientFactory httpClientFactory)
    : ICommandHandler<CombineSectionsToPaperCommand, CombineSectionsToPaperResult>
{
    public async Task<CombineSectionsToPaperResult> Handle(CombineSectionsToPaperCommand request,
        CancellationToken cancellationToken)
    {
        var role = await managementApiService.GetMyProjectRoleAsync(request.Dto.ProjectId, cancellationToken);
        if (role.IsNullOrEmpty() || !AuthorizeConstants.PaperAuthor.EqualsIgnoreCase(role))
            throw new UnauthorizedException(MessageCode.AccessDenied);
        await session.BeginTransactionAsync(cancellationToken);

        var paper = await session.LoadAsync<PaperEntity>(request.PaperId, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.PaperIsNotExists, request.PaperId.ToString());

        var mainSections = await session.Query<SectionEntity>()
            .Where(x => x.PaperId == request.PaperId &&
                        x.IsMainSection != null &&
                        x.IsMainSection == true)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);

        var referenceSection = mainSections
            .Where(x => x.Title!.EqualsIgnoreCase(SectionConstants.ReferencesTitle) ||
                        x.Title!.EqualsIgnoreCase(SectionConstants.ReferenceTitle))
            .FirstOrDefault();

        var bodySections = mainSections
            .Where(x => x.Id != referenceSection!.Id)
            .ToList();

        var combineSectionPackages = BuildPackagesBlock(mainSections);

        var referenceSectionContent = referenceSection?.Content?.Trim() ?? string.Empty;

        var bodyContent = string.Join(
            $"{Environment.NewLine}{Environment.NewLine}",
            bodySections
                .Select(x => x.Content)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x)));

        var content = BuildTemplateContent(paper.Title, author: string.Empty, combineSectionPackages,
            referenceSectionContent,
            bodyContent);

        var journal = await session.LoadAsync<ConferenceJournalEntity>(paper.ConferenceJournalId!, cancellationToken);

        // Format the paper content to match the conference/journal template style using AI
        string savedContent;
        if (!string.IsNullOrWhiteSpace(journal?.TexFile))
        {
            var httpClient = httpClientFactory.CreateClient();
            var templateContent = await httpClient.GetStringAsync(journal.TexFile, cancellationToken);
            savedContent = await aiApiService.FormatPaperToStyleAsync(content, templateContent, cancellationToken);
        }
        else
        {
            savedContent = BuildIEEEtranTemplateContent(paper.Title, author: string.Empty, combineSectionPackages,
                referenceSectionContent,
                bodyContent,
                journal?.Type);
        }


        var files = mainSections
            .Where(x => x.Files != null)
            .SelectMany(x => x.Files!)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct()
            .ToList();

        var versionNumber = await session.Query<PaperVersionEntity>()
            .CountAsync(x => x.PaperId == paper.Id, cancellationToken) + 1;
        var name = $"Version {versionNumber}";

        var version = PaperVersionEntity.Create(
            id: Guid.NewGuid(),
            paperId: paper.Id,
            name: name,
            content: savedContent,
            references: referenceSection!.References,
            files: files,
            createdBy: request.UserName);

        session.Store(version);
        await session.SaveChangesAsync(cancellationToken);

        return new CombineSectionsToPaperResult
        {
            Version = new PaperVersionInfo
            {
                Id = version.Id,
                Name = version.Name,
                Content = version.Content,
                References = version.References,
                Files = version.Files,
                CreatedBy = version.CreatedBy,
                CreatedOnUtc = version.CreatedOnUtc,
                LastModifiedBy = version.LastModifiedBy,
                LastModifiedOnUtc = version.LastModifiedOnUtc ?? version.CreatedOnUtc
            }
        };
    }

    private static string BuildPackagesBlock(IEnumerable<SectionEntity> sections)
    {
        return string.Join(
            Environment.NewLine,
            sections
                .Where(x => x.Packages != null)
                .SelectMany(x => x.Packages!)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct());
    }

    private static string BuildTemplateContent(
        string title,
        string author,
        string combineSectionPackages,
        string referenceSectionContent,
        string bodyContent)
    {
        var titleBlock = $"\\title{{{title}}}";
        var authorBlock = $"\\author{{{author}}}";

        var blocks = new List<string>
        {
            "\\documentclass{article}",
            combineSectionPackages,
            titleBlock,
            authorBlock,
            "\\begin{document}",
            "\\maketitle",
            bodyContent,
            referenceSectionContent,
            "\\printbibliography",
            "\\end{document}"
        };

        return string.Join(
            $"{Environment.NewLine}{Environment.NewLine}",
            blocks.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private static string BuildIEEEtranTemplateContent(
        string title,
        string author,
        string combineSectionPackages,
        string referenceSectionContent,
        string bodyContent,
        ConferenceJournalType? journalType)
    {
        var documentClass = journalType switch
        {
            ConferenceJournalType.Journal    => "\\documentclass[journal]{IEEEtran}",
            ConferenceJournalType.Conference => "\\documentclass[conference]{IEEEtran}",
            _                                => "\\documentclass[journal]{IEEEtran}"
        };

        var titleBlock = $"\\title{{{title}}}";
        var authorBlock = $"\\author{{{author}}}";

        var blocks = new List<string>
        {
            documentClass,
            "\\usepackage{amsmath, amssymb}",
            "\\usepackage{graphicx}",
            "\\usepackage{booktabs}",
            "\\usepackage{hyperref}",
            "\\usepackage[style=ieee, backend=biber]{biblatex}",
            combineSectionPackages,
            titleBlock,
            authorBlock,
            "\\begin{document}",
            "\\maketitle",
            bodyContent,
            referenceSectionContent,
            "\\printbibliography",
            "\\end{document}"
        };

        return string.Join(
            $"{Environment.NewLine}{Environment.NewLine}",
            blocks.Where(x => !string.IsNullOrWhiteSpace(x)));
    }
}