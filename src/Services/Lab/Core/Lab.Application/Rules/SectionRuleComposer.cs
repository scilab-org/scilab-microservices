using System.Text;
using Lab.Application.Dtos.Papers;
using Lab.Application.Services;
using Lab.Domain.Entities;
using PaperRules = Lab.Domain.Constants.Rules;

namespace Lab.Application.Rules;

public static class SectionRuleComposer
{
    public static string BuildProjectRule(ManagementProjectInfo? project)
    {
        var contentBuilder = new StringBuilder();

        contentBuilder.AppendLine("# Project: **Level 3 (Guideline)**");
        contentBuilder.AppendLine();

        AppendBlock(contentBuilder, "Context", project?.Context);
        AppendBlock(contentBuilder, "Keypoint", project?.Keypoint);
        AppendBlock(contentBuilder, "Domain", project?.Domain);

        return contentBuilder.ToString().TrimEnd();
    }

    public static string BuildPaperRule(CreatePaperDto paperDto, ConferenceJournalEntity journal)
    {
        return BuildPaperRule(
            paperDto.Context,
            paperDto.Abstract,
            paperDto.ResearchGap,
            paperDto.GapType,
            paperDto.ResearchAim,
            journal);
    }

    public static string BuildPaperRule(PaperEntity paper, ConferenceJournalEntity journal)
    {
        return BuildPaperRule(
            paper.Context,
            paper.Abstract,
            paper.ResearchGap,
            paper.GapType,
            paper.ResearchAim,
            journal);
    }

    public static string BuildSectionRule(string? title, string? guideline, string? mainIdea = null)
    {
        var contentBuilder = new StringBuilder();

        contentBuilder.AppendLine($"# Section {title}: **Level 1 (Critical)**");
        contentBuilder.AppendLine();

        AppendBlock(contentBuilder, "Rule", guideline);
        AppendBlock(contentBuilder, "Main Idea", mainIdea);

        return contentBuilder.ToString().TrimEnd();
    }

    public static string ComposeNormalizedRule(
        string? projectRule,
        string? paperRule,
        string? sectionRule)
    {
        var contentBuilder = new StringBuilder();

        contentBuilder.AppendLine("## Rule Level to write research paper");
        contentBuilder.AppendLine("**Level 1 (Critical):** Must follow strictly");
        contentBuilder.AppendLine("**Level 2 (Important):** Should follow");
        contentBuilder.AppendLine("**Level 3 (Guidelines):** Consider Context");
        contentBuilder.AppendLine();

        AppendRawBlock(contentBuilder, projectRule);
        AppendRawBlock(contentBuilder, paperRule);
        AppendRawBlock(contentBuilder, sectionRule);

        return contentBuilder.ToString().TrimEnd();
    }

    private static string BuildPaperRule(
        string? context,
        string? abstractText,
        string? researchGap,
        string? gapType,
        string? researchAim,
        ConferenceJournalEntity journal)
    {
        var contentBuilder = new StringBuilder();

        contentBuilder.AppendLine("# Paper: **Level 2 (Important)**");
        contentBuilder.AppendLine();

        AppendBlock(contentBuilder, "Context", context);
        AppendBlock(contentBuilder, "Abstract", abstractText);
        AppendBlock(contentBuilder, "Research Gap", researchGap);
        AppendBlock(contentBuilder, "Gap Type", gapType);
        AppendBlock(contentBuilder, "Research Aim", researchAim);
        AppendBlock(contentBuilder, "Journal Name", journal.Name);
        AppendBlock(contentBuilder, "Journal Style", journal.Style);
        AppendBlock(contentBuilder, "Rule", PaperRules.Paper);

        return contentBuilder.ToString().TrimEnd();
    }

    // ── Section Context builders (cached per sub-level, like Rule) ────────

    public static string BuildProjectContext(ManagementProjectInfo? project)
    {
        var contentBuilder = new StringBuilder();
        contentBuilder.AppendLine("# Project Context: **Level 3 (Guidelines)**");
        contentBuilder.AppendLine();
        AppendBlock(contentBuilder, "Context", project?.Context);
        return contentBuilder.ToString().TrimEnd();
    }

    public static string BuildPaperContext(PaperEntity paper)
    {
        var contentBuilder = new StringBuilder();
        contentBuilder.AppendLine("# Paper Context: **Level 2 (Important)**");
        contentBuilder.AppendLine();
        AppendBlock(contentBuilder, "Context", paper.Context);
        AppendBlock(contentBuilder, "Research Gap", paper.ResearchGap);
        AppendBlock(contentBuilder, "Research Aim", paper.ResearchAim);
        return contentBuilder.ToString().TrimEnd();
    }

    public static string BuildPaperContext(CreatePaperDto paperDto)
    {
        var contentBuilder = new StringBuilder();
        contentBuilder.AppendLine("# Paper Context: **Level 2 (Important)**");
        contentBuilder.AppendLine();
        AppendBlock(contentBuilder, "Context", paperDto.Context);
        AppendBlock(contentBuilder, "Research Gap", paperDto.ResearchGap);
        AppendBlock(contentBuilder, "Research Aim", paperDto.ResearchAim);
        return contentBuilder.ToString().TrimEnd();
    }

    public static string ComposeSectionContext(
        string? projectContext,
        string? paperContext,
        string? sectionTitle,
        string? sectionMainIdea)
    {
        var contentBuilder = new StringBuilder();

        contentBuilder.AppendLine("## Context Levels");
        contentBuilder.AppendLine("**Level 1 (Critical):** Section-specific context");
        contentBuilder.AppendLine("**Level 2 (Important):** Paper-specific context");
        contentBuilder.AppendLine("**Level 3 (Guidelines):** Project-level context");
        contentBuilder.AppendLine();

        AppendRawBlock(contentBuilder, projectContext);
        AppendRawBlock(contentBuilder, paperContext);

        // Section context — Level 1 (built inline, not cached separately)
        contentBuilder.AppendLine($"# Section {sectionTitle} Context: **Level 1 (Critical)**");
        contentBuilder.AppendLine();
        AppendBlock(contentBuilder, "Main Idea", sectionMainIdea);

        return contentBuilder.ToString().TrimEnd();
    }

    private static void AppendBlock(StringBuilder contentBuilder, string title, string? content)
    {
        contentBuilder.AppendLine($"## {title}");
        if (!string.IsNullOrWhiteSpace(content))
            contentBuilder.AppendLine(content);
        contentBuilder.AppendLine();
    }

    private static void AppendRawBlock(StringBuilder contentBuilder, string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return;

        contentBuilder.AppendLine(content.TrimEnd());
        contentBuilder.AppendLine();
    }
}