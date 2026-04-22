namespace Lab.Application.Tests.Rules;

public sealed class SectionRuleComposerTests
{
    #region BuildProjectRule

    [Fact]
    public void BuildProjectRule_ShouldContainAllFields_WhenProjectHasAllValues()
    {
        var project = new ManagementProjectInfo(
            Guid.NewGuid(), "Project", "P001", "Desc", "Active",
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(30),
            "AI Context", "CS Domain", "Key point");

        var result = SectionRuleComposer.BuildProjectRule(project);

        result.Should().Contain("Level 3 (Guideline)");
        result.Should().Contain("AI Context");
        result.Should().Contain("CS Domain");
        result.Should().Contain("Key point");
    }

    [Fact]
    public void BuildProjectRule_ShouldHandleNullProject()
    {
        var result = SectionRuleComposer.BuildProjectRule(null);
        result.Should().Contain("Level 3 (Guideline)");
    }

    [Fact]
    public void BuildProjectRule_ShouldHandleNullFields()
    {
        var project = new ManagementProjectInfo(
            Guid.Empty, null, null, null, null, null, null, null, null, null);
        var result = SectionRuleComposer.BuildProjectRule(project);
        result.Should().Contain("Level 3 (Guideline)");
    }

    #endregion

    #region BuildPaperRule (DTO overload)

    [Fact]
    public void BuildPaperRule_FromDto_ShouldContainAllFields()
    {
        var dto = new CreatePaperDto
        {
            Context = "AI Research",
            Abstract = "Abstract text",
            ResearchGap = "Gap text",
            GapType = "Methodological",
            ResearchAim = "Aim text",
            ConferenceJournalId = Guid.NewGuid(),
            ConferenceJournalName = "ICSE"
        };
        var journal = ConferenceJournalEntity.Create(
            Guid.NewGuid(), "ICSE 2024", "A*", null, null, "IEEE", ConferenceJournalType.Journal, [], null, null);

        var result = SectionRuleComposer.BuildPaperRule(dto, journal);

        result.Should().Contain("Level 2 (Important)");
        result.Should().Contain("AI Research");
        result.Should().Contain("Abstract text");
        result.Should().Contain("Gap text");
        result.Should().Contain("Methodological");
        result.Should().Contain("Aim text");
        result.Should().Contain("ICSE 2024");
        result.Should().Contain("IEEE");
    }

    #endregion

    #region BuildPaperRule (Entity overload)

    [Fact]
    public void BuildPaperRule_FromEntity_ShouldContainAllFields()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "Title",
            context: "Context", abstractText: "Abstract",
            researchGap: "Gap", gapType: "Type", researchAim: "Aim");
        var journal = ConferenceJournalEntity.Create(
            Guid.NewGuid(), "Journal", "B", null, null, "ACM", ConferenceJournalType.Journal, [], null, null);

        var result = SectionRuleComposer.BuildPaperRule(paper, journal);

        result.Should().Contain("Level 2 (Important)");
        result.Should().Contain("Context");
        result.Should().Contain("Abstract");
        result.Should().Contain("Gap");
        result.Should().Contain("Aim");
        result.Should().Contain("Journal");
        result.Should().Contain("ACM");
    }

    #endregion

    #region BuildSectionRule

    [Fact]
    public void BuildSectionRule_ShouldContainTitleAndGuideline()
    {
        var result = SectionRuleComposer.BuildSectionRule("Introduction", "Write clearly", "Main idea here");

        result.Should().Contain("Level 1 (Critical)");
        result.Should().Contain("Introduction");
        result.Should().Contain("Write clearly");
        result.Should().Contain("Main idea here");
    }

    [Fact]
    public void BuildSectionRule_ShouldHandleNullMainIdea()
    {
        var result = SectionRuleComposer.BuildSectionRule("Methods", "Be specific");
        result.Should().Contain("Methods");
        result.Should().Contain("Be specific");
    }

    [Fact]
    public void BuildSectionRule_ShouldHandleNullGuideline()
    {
        var result = SectionRuleComposer.BuildSectionRule("Results", null);
        result.Should().Contain("Results");
    }

    #endregion

    #region ComposeNormalizedRule

    [Fact]
    public void ComposeNormalizedRule_ShouldCombineAllRules()
    {
        var projectRule = "Project rule content";
        var paperRule = "Paper rule content";
        var sectionRule = "Section rule content";

        var result = SectionRuleComposer.ComposeNormalizedRule(projectRule, paperRule, sectionRule);

        result.Should().Contain("Rule Level to write research paper");
        result.Should().Contain("Level 1 (Critical)");
        result.Should().Contain("Level 2 (Important)");
        result.Should().Contain("Level 3 (Guidelines)");
        result.Should().Contain("Project rule content");
        result.Should().Contain("Paper rule content");
        result.Should().Contain("Section rule content");
    }

    [Fact]
    public void ComposeNormalizedRule_ShouldSkipNullOrWhitespaceBlocks()
    {
        var result = SectionRuleComposer.ComposeNormalizedRule(null, null, null);
        result.Should().Contain("Rule Level to write research paper");
    }

    [Fact]
    public void ComposeNormalizedRule_ShouldSkipEmptyBlocks()
    {
        var result = SectionRuleComposer.ComposeNormalizedRule("", "  ", "");
        result.Should().Contain("Rule Level to write research paper");
    }

    #endregion

    #region BuildProjectContext

    [Fact]
    public void BuildProjectContext_ShouldContainContext()
    {
        var project = new ManagementProjectInfo(
            Guid.NewGuid(), "P", "C", null, null, null, null, "Project context", null, null);

        var result = SectionRuleComposer.BuildProjectContext(project);

        result.Should().Contain("Level 3 (Guidelines)");
        result.Should().Contain("Project context");
    }

    [Fact]
    public void BuildProjectContext_ShouldHandleNull()
    {
        var result = SectionRuleComposer.BuildProjectContext(null);
        result.Should().Contain("Level 3 (Guidelines)");
    }

    #endregion

    #region BuildPaperContext (Entity overload)

    [Fact]
    public void BuildPaperContext_FromEntity_ShouldContainFields()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "Title",
            context: "Paper ctx", researchGap: "Gap", researchAim: "Aim");

        var result = SectionRuleComposer.BuildPaperContext(paper);

        result.Should().Contain("Level 2 (Important)");
        result.Should().Contain("Paper ctx");
        result.Should().Contain("Gap");
        result.Should().Contain("Aim");
    }

    #endregion

    #region BuildPaperContext (DTO overload)

    [Fact]
    public void BuildPaperContext_FromDto_ShouldContainFields()
    {
        var dto = new CreatePaperDto
        {
            Context = "DTO ctx",
            ResearchGap = "DTO gap",
            ResearchAim = "DTO aim",
            ConferenceJournalId = Guid.NewGuid(),
            ConferenceJournalName = "J"
        };

        var result = SectionRuleComposer.BuildPaperContext(dto);

        result.Should().Contain("Level 2 (Important)");
        result.Should().Contain("DTO ctx");
        result.Should().Contain("DTO gap");
        result.Should().Contain("DTO aim");
    }

    #endregion

    #region ComposeSectionContext

    [Fact]
    public void ComposeSectionContext_ShouldCombineAllContexts()
    {
        var result = SectionRuleComposer.ComposeSectionContext(
            "Project ctx", "Paper ctx", "Introduction", "Main idea");

        result.Should().Contain("Context Levels");
        result.Should().Contain("Level 1 (Critical)");
        result.Should().Contain("Level 2 (Important)");
        result.Should().Contain("Level 3 (Guidelines)");
        result.Should().Contain("Project ctx");
        result.Should().Contain("Paper ctx");
        result.Should().Contain("Introduction");
        result.Should().Contain("Main idea");
    }

    [Fact]
    public void ComposeSectionContext_ShouldSkipNullContexts()
    {
        var result = SectionRuleComposer.ComposeSectionContext(null, null, "Methods", null);
        result.Should().Contain("Methods");
    }

    #endregion
}
