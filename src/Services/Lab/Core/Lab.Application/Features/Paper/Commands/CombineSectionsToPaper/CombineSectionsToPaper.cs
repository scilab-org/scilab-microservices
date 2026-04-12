using JasperFx.Core;
using Lab.Application.Dtos.Papers;
using Lab.Application.Models.Results;
using Lab.Application.Services;
using Lab.Domain.Constants;
using Lab.Domain.Entities;
using Lab.Domain.Models;
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

public class CombineSectionsToPaperCommandHandler(IDocumentSession session, IManagementApiService managementApiService)
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

        var content = BuildTemplateContent(combineSectionPackages, referenceSectionContent,
            bodyContent);


        Combine? combine = null;

        if (!request.Dto.IsPreview)
        {
            var name = $"Version {paper.Combines.Count + 1}";
            var savedContent = content;
            if (request.Dto.Content != null)
                savedContent = request.Dto.Content.Trim();

            combine = paper.AddCombineVersion(
                name: name,
                content: savedContent,
                reference: referenceSection!.References,
                createdBy: request.UserName
            );

            session.Update(paper);
            await session.SaveChangesAsync(cancellationToken);
        }

        if (combine == null)
            return new CombineSectionsToPaperResult
            {
                Combine = new PaperCombineInfo
                {
                    Id = Guid.Empty,
                    Name = "Preview Version",
                    Content = content,
                    References = referenceSection?.References,
                    IsSave = false,
                    CreatedBy = request.UserName,
                    CreatedOnUtc = DateTimeOffset.UtcNow,
                    LastModifiedBy = request.UserName,
                    LastModifiedOnUtc = DateTimeOffset.UtcNow
                }
            };

        return new CombineSectionsToPaperResult
        {
            Combine = new PaperCombineInfo
            {
                Id = combine.Id,
                Name = combine.Name,
                Content = combine.Content,
                References = combine.References,
                IsSave = true,
                CreatedBy = combine.CreatedBy,
                CreatedOnUtc = combine.CreatedOnUtc,
                LastModifiedBy = combine.LastModifiedBy,
                LastModifiedOnUtc = combine.LastModifiedOnUtc
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
        string combineSectionPackages,
        string referenceSectionContent,
        string bodyContent)
    {
        var blocks = new List<string>
        {
            "\\documentclass{article}",
            combineSectionPackages,
            referenceSectionContent,
            "\\begin{document}",
            bodyContent,
            "\\printbibliography",
            "\\end{document}"
        };

        return string.Join(
            $"{Environment.NewLine}{Environment.NewLine}",
            blocks.Where(x => !string.IsNullOrWhiteSpace(x)));
    }
}