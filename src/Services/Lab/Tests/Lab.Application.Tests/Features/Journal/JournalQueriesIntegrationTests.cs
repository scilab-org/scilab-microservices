using Lab.Application.Features.Journal.Queries.GetJournalById;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;
using Lab.Domain.Enums;

namespace Lab.Application.Tests.Features.Journal;

public class JournalQueriesIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "journal_queries_tests";

    private readonly Mock<IManagementApiService> _mockMgmt = new();

    private static ManagementProjectInfo FakeProject(Guid id) =>
        new(id, "Test Project", "TP", "Desc", "Active", null, null, "Context", "AI", "KP");

    // ─── GetJournalByIdQueryHandler ───────────────────────────────────────────

    [Fact]
    public async Task GetJournalById_WithNonExistentId_ShouldThrowNotFoundException()
    {
        var handler = new GetJournalByIdQueryHandler(Session, _mockMgmt.Object, Mapper);

        var act = () => handler.Handle(new GetJournalByIdQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetJournalById_WithExistingJournal_NoAssociations_ShouldReturnMappedResult()
    {
        var journal = ConferenceJournalEntity.Create(
            Guid.NewGuid(), "IEEE Trans", "Q1", "https://ieee.org",
            "1234-5678", null, ConferenceJournalType.Journal,
            new List<Guid>(), null, null);
        Session.Store(journal);
        await Session.SaveChangesAsync();

        var handler = new GetJournalByIdQueryHandler(Session, _mockMgmt.Object, Mapper);
        var result = await handler.Handle(new GetJournalByIdQuery(journal.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Journal.Name.Should().Be("IEEE Trans");
        result.Projects.Should().BeEmpty();
        result.Papers.Should().BeEmpty();
    }

    [Fact]
    public async Task GetJournalById_WithTemplates_ShouldPopulateTemplates()
    {
        var template = new TemplateEntity
        {
            Id = Guid.NewGuid(),
            Code = "TPL-001",
            Description = "Test template",
            CreatedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(template);

        var journal = ConferenceJournalEntity.Create(
            Guid.NewGuid(), "ACM Computing", "Q2", null, null, null,
            ConferenceJournalType.Conference,
            new List<Guid> { template.Id }, null, null);
        Session.Store(journal);
        await Session.SaveChangesAsync();

        var handler = new GetJournalByIdQueryHandler(Session, _mockMgmt.Object, Mapper);
        var result = await handler.Handle(new GetJournalByIdQuery(journal.Id), CancellationToken.None);

        result.Journal.Templates.Should().HaveCount(1);
        result.Journal.Templates[0].Code.Should().Be("TPL-001");
    }

    [Fact]
    public async Task GetJournalById_WithProjects_ShouldPopulateProjects()
    {
        var projectId = Guid.NewGuid();
        var journal = ConferenceJournalEntity.Create(
            Guid.NewGuid(), "Nature", "Q1", null, null, null,
            ConferenceJournalType.Journal,
            new List<Guid>(), null, null,
            projectIds: new List<Guid> { projectId });
        Session.Store(journal);
        await Session.SaveChangesAsync();

        _mockMgmt.Setup(x => x.GetProjectsByIdsAsync(
                It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ManagementProjectInfo> { FakeProject(projectId) });

        var handler = new GetJournalByIdQueryHandler(Session, _mockMgmt.Object, Mapper);
        var result = await handler.Handle(new GetJournalByIdQuery(journal.Id), CancellationToken.None);

        result.Projects.Should().HaveCount(1);
        result.Projects[0].Name.Should().Be("Test Project");
        result.Projects[0].Code.Should().Be("TP");
    }

    [Fact]
    public async Task GetJournalById_WithProjectIds_WhenProjectNotReturnedByService_ShouldReturnEmpty()
    {
        var projectId = Guid.NewGuid();
        var journal = ConferenceJournalEntity.Create(
            Guid.NewGuid(), "Science", "Q1", null, null, null,
            ConferenceJournalType.Journal,
            new List<Guid>(), null, null,
            projectIds: new List<Guid> { projectId });
        Session.Store(journal);
        await Session.SaveChangesAsync();

        _mockMgmt.Setup(x => x.GetProjectsByIdsAsync(
                It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ManagementProjectInfo>()); // empty - project not found in management

        var handler = new GetJournalByIdQueryHandler(Session, _mockMgmt.Object, Mapper);
        var result = await handler.Handle(new GetJournalByIdQuery(journal.Id), CancellationToken.None);

        result.Projects.Should().BeEmpty();
    }

    [Fact]
    public async Task GetJournalById_WithPapers_ShouldPopulatePapers()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "Research Paper on AI");
        Session.Store(paper);

        var journal = ConferenceJournalEntity.Create(
            Guid.NewGuid(), "AI Journal", "Q1", null, null, null,
            ConferenceJournalType.Journal,
            new List<Guid>(), null, null,
            paperIds: new List<Guid> { paper.Id });
        Session.Store(journal);
        await Session.SaveChangesAsync();

        var handler = new GetJournalByIdQueryHandler(Session, _mockMgmt.Object, Mapper);
        var result = await handler.Handle(new GetJournalByIdQuery(journal.Id), CancellationToken.None);

        result.Papers.Should().HaveCount(1);
        result.Papers[0].Title.Should().Be("Research Paper on AI");
    }

    [Fact]
    public async Task GetJournalById_WithPaperIdNotInDb_ShouldReturnEmptyPapers()
    {
        var missingPaperId = Guid.NewGuid(); // not stored in session

        var journal = ConferenceJournalEntity.Create(
            Guid.NewGuid(), "Orphan Journal", "Q3", null, null, null,
            ConferenceJournalType.Journal,
            new List<Guid>(), null, null,
            paperIds: new List<Guid> { missingPaperId });
        Session.Store(journal);
        await Session.SaveChangesAsync();

        var handler = new GetJournalByIdQueryHandler(Session, _mockMgmt.Object, Mapper);
        var result = await handler.Handle(new GetJournalByIdQuery(journal.Id), CancellationToken.None);

        result.Papers.Should().BeEmpty();
    }
}
