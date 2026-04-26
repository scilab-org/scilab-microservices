using Lab.Application.Features.Journal.Queries.GetJournals;
using Lab.Application.Features.Journal.Queries.GetJournalById;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;
using Lab.Domain.Enums;

namespace Lab.Application.Tests.Features.Journal;

public class JournalIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "journal_tests";

    private ConferenceJournalEntity SeedJournal(string name = "IEEE", List<Guid>? templateIds = null,
        List<Guid>? projectIds = null, List<Guid>? paperIds = null)
    {
        var entity = ConferenceJournalEntity.Create(Guid.NewGuid(), name, "Q1", "https://ieee.org",
            "1234-5678", null, ConferenceJournalType.Journal,
            templateIds ?? new List<Guid>(), null, null,
            projectIds: projectIds, paperIds: paperIds);
        Session.Store(entity);
        return entity;
    }

    #region GetJournals

    [Fact]
    public async Task GetJournals_WithNoFilter_ShouldReturnAll()
    {
        SeedJournal("IEEE"); SeedJournal("ACM");
        await Session.SaveChangesAsync();

        var handler = new GetJournalsQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetJournalsQuery(new GetJournalsFilter(), new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetJournals_WithNameFilter_ShouldReturnMatching()
    {
        SeedJournal("IEEE Trans"); SeedJournal("ACM Computing");
        await Session.SaveChangesAsync();

        var handler = new GetJournalsQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetJournalsQuery(new GetJournalsFilter { Name = "IEEE" }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetJournals_WithTemplateFilter_ShouldReturnMatching()
    {
        var templateId = Guid.NewGuid();
        SeedJournal("With Template", templateIds: new List<Guid> { templateId });
        SeedJournal("Without Template");
        await Session.SaveChangesAsync();

        var handler = new GetJournalsQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetJournalsQuery(new GetJournalsFilter { TemplateId = templateId }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetJournals_WithTypeFilter_ShouldReturnMatching()
    {
        SeedJournal("Journal");
        var conf = ConferenceJournalEntity.Create(Guid.NewGuid(), "Conference", null, null, null, null,
            ConferenceJournalType.Conference, new List<Guid>(), null, null);
        Session.Store(conf);
        await Session.SaveChangesAsync();

        var handler = new GetJournalsQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetJournalsQuery(new GetJournalsFilter { Type = ConferenceJournalType.Conference }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetJournals_WithTemplateData_ShouldPopulateTemplateDtos()
    {
        var template = new TemplateEntity
        {
            Id = Guid.NewGuid(), Code = "TPL-001",
            Description = "Test", CreatedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(template);
        SeedJournal("With Template Info", templateIds: new List<Guid> { template.Id });
        await Session.SaveChangesAsync();

        var handler = new GetJournalsQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetJournalsQuery(new GetJournalsFilter(), new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Templates.Should().HaveCount(1);
        result.Items[0].Templates![0].Code.Should().Be("TPL-001");
    }

    #endregion

    #region GetJournalInProjectById

    [Fact]
    public async Task GetJournalInProjectById_ShouldReturnStubResult()
    {
        var handler = new GetJournalInProjectByIdQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetJournalInProjectByIdQuery(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.Should().NotBeNull();
    }

    #endregion

    #region GetJournalsInProject

    [Fact]
    public async Task GetJournalsInProject_ShouldReturnStubResult()
    {
        var handler = new GetJournalsInProjectQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetJournalsInProjectQuery(new GetJournalsFilter(), new PaginationRequest(1, 10), Guid.NewGuid()),
            CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }

    #endregion
}
