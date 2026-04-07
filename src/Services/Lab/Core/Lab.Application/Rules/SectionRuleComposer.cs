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

    public static string BuildPaperRule(CreatePaperDto paperDto)
    {
        return BuildPaperRule(
            paperDto.Context,
            paperDto.ResearchGap,
            paperDto.GapType,
            paperDto.Journal.StyleRule);
    }

    public static string BuildPaperRule(PaperEntity paper)
    {
        return BuildPaperRule(
            paper.Context,
            paper.ResearchGap,
            paper.GapType,
            paper.StyleRule);
    }

    public static string BuildSectionRule(string? title, string? guideline)
    {
        var contentBuilder = new StringBuilder();

        contentBuilder.AppendLine($"# Section {title}: **Level 1 (Critical)**");
        contentBuilder.AppendLine();

        AppendBlock(contentBuilder, "Rule", guideline);

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
        string? researchGap,
        string? gapType,
        string? journalStyleRule)
    {
        var contentBuilder = new StringBuilder();

        contentBuilder.AppendLine("# Paper: **Level 2 (Important)**");
        contentBuilder.AppendLine();

        AppendBlock(contentBuilder, "Context", context);
        AppendBlock(contentBuilder, "Research Gap", researchGap);
        AppendBlock(contentBuilder, "Gap Type", gapType);
        AppendBlock(contentBuilder, "Rule", PaperRules.Paper);
        AppendBlock(contentBuilder, "Journal Style", journalStyleRule);

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