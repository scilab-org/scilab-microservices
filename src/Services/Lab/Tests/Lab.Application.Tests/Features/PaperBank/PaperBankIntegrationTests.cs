using Lab.Application.Features.PaperBank.Commands.UpdatePaperBankIngestionStatus;
using Lab.Application.Features.PaperBank.Queries.GetPaperBankById;
using Lab.Application.Features.PaperBank.Queries.GetPaperBanks;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;
using Lab.Domain.Enums;

namespace Lab.Application.Tests.Features.PaperBank;

public class PaperBankIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "paper_bank_tests";

    private PaperBankEntity SeedPaperBank(string title = "Sample Paper", Guid? journalId = null, List<Guid>? gapTypeIds = null)
    {
        var entity = PaperBankEntity.Create(Guid.NewGuid(), title,
            conferenceJournalId: journalId, gapTypeIds: gapTypeIds);
        Session.Store(entity);
        return entity;
    }

    #region UpdatePaperBankIngestionStatus

    [Fact]
    public async Task UpdateIngestionStatus_Success_ShouldUpdateEntity()
    {
        var pb = SeedPaperBank();
        await Session.SaveChangesAsync();

        var handler = new UpdatePaperBankIngestionStatusHandler(Session);
        var result = await handler.Handle(
            new UpdatePaperBankIngestionStatusCommand(pb.Id, true, null), CancellationToken.None);

        result.Should().Be(pb.Id);
        var updated = await Session.LoadAsync<PaperBankEntity>(pb.Id);
        updated!.IsIngested.Should().BeTrue();
        updated.IngestStatus.Should().Be(IngestStatus.Success);
    }

    [Fact]
    public async Task UpdateIngestionStatus_Failure_ShouldSetFailedStatus()
    {
        var pb = SeedPaperBank();
        await Session.SaveChangesAsync();

        var handler = new UpdatePaperBankIngestionStatusHandler(Session);
        await handler.Handle(
            new UpdatePaperBankIngestionStatusCommand(pb.Id, false, "parse error"), CancellationToken.None);

        var updated = await Session.LoadAsync<PaperBankEntity>(pb.Id);
        updated!.IsIngested.Should().BeFalse();
        updated.IngestStatus.Should().Be(IngestStatus.Failed);
    }

    [Fact]
    public async Task UpdateIngestionStatus_WithNonExistentId_ShouldThrow()
    {
        var handler = new UpdatePaperBankIngestionStatusHandler(Session);
        var act = () => handler.Handle(
            new UpdatePaperBankIngestionStatusCommand(Guid.NewGuid(), true, null), CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region GetPaperBankById

    [Fact]
    public async Task GetPaperBankById_WithExisting_ShouldReturnMappedResult()
    {
        var pb = SeedPaperBank("My Paper");
        await Session.SaveChangesAsync();

        var handler = new GetPaperBankByIdQueryHandler(Session, Mapper);
        var result = await handler.Handle(new GetPaperBankByIdQuery(pb.Id), CancellationToken.None);

        result.PaperBank.Should().NotBeNull();
        result.PaperBank.Title.Should().Be("My Paper");
    }

    [Fact]
    public async Task GetPaperBankById_WithJournal_ShouldPopulateJournalName()
    {
        var journal = ConferenceJournalEntity.Create(Guid.NewGuid(), "IEEE", null, null, null, null,
            ConferenceJournalType.Journal, new List<Guid>(), null, null);
        Session.Store(journal);
        var pb = SeedPaperBank("With Journal", journalId: journal.Id);
        await Session.SaveChangesAsync();

        var handler = new GetPaperBankByIdQueryHandler(Session, Mapper);
        var result = await handler.Handle(new GetPaperBankByIdQuery(pb.Id), CancellationToken.None);

        result.PaperBank.ConferenceJournalName.Should().Be("IEEE");
    }

    [Fact]
    public async Task GetPaperBankById_WithGapTypes_ShouldPopulateGapTypes()
    {
        var gt = new GapTypeEntity { Id = Guid.NewGuid(), Name = "Empirical Gap" };
        Session.Store(gt);
        var pb = SeedPaperBank("With Gaps", gapTypeIds: new List<Guid> { gt.Id });
        await Session.SaveChangesAsync();

        var handler = new GetPaperBankByIdQueryHandler(Session, Mapper);
        var result = await handler.Handle(new GetPaperBankByIdQuery(pb.Id), CancellationToken.None);

        result.PaperBank.GapTypes.Should().HaveCount(1);
        result.PaperBank.GapTypes![0].Name.Should().Be("Empirical Gap");
    }

    [Fact]
    public async Task GetPaperBankById_WithNonExistent_ShouldThrowNotFoundException()
    {
        var handler = new GetPaperBankByIdQueryHandler(Session, Mapper);
        var act = () => handler.Handle(new GetPaperBankByIdQuery(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetPaperBanks

    [Fact]
    public async Task GetPaperBanks_WithNoFilter_ShouldReturnAll()
    {
        SeedPaperBank("A"); SeedPaperBank("B");
        await Session.SaveChangesAsync();

        var handler = new GetPaperBanksQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperBanksQuery(new GetPaperBanksFilter(), new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPaperBanks_WithTitleFilter_ShouldReturnMatching()
    {
        SeedPaperBank("Machine Learning Paper"); SeedPaperBank("Data Mining");
        await Session.SaveChangesAsync();

        var handler = new GetPaperBanksQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperBanksQuery(new GetPaperBanksFilter { Title = "machine" }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPaperBanks_WithAuthorFilter_ShouldReturnMatching()
    {
        var pb = PaperBankEntity.Create(Guid.NewGuid(), "Paper 1", authors: "John Smith");
        var pb2 = PaperBankEntity.Create(Guid.NewGuid(), "Paper 2", authors: "Jane Doe");
        Session.Store(pb); Session.Store(pb2);
        await Session.SaveChangesAsync();

        var handler = new GetPaperBanksQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperBanksQuery(new GetPaperBanksFilter { Author = new[] { "john" } }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPaperBanks_WithPublisherFilter_ShouldReturnMatching()
    {
        var pb = PaperBankEntity.Create(Guid.NewGuid(), "Paper 1", publisher: "Elsevier");
        var pb2 = PaperBankEntity.Create(Guid.NewGuid(), "Paper 2", publisher: "Springer");
        Session.Store(pb); Session.Store(pb2);
        await Session.SaveChangesAsync();

        var handler = new GetPaperBanksQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperBanksQuery(new GetPaperBanksFilter { Publisher = "Elsevier" }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPaperBanks_WithDoiFilter_ShouldReturnMatching()
    {
        var pb = PaperBankEntity.Create(Guid.NewGuid(), "Paper 1", doi: "10.1234/test");
        var pb2 = PaperBankEntity.Create(Guid.NewGuid(), "Paper 2", doi: "10.5678/other");
        Session.Store(pb); Session.Store(pb2);
        await Session.SaveChangesAsync();

        var handler = new GetPaperBanksQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperBanksQuery(new GetPaperBanksFilter { Doi = "1234" }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPaperBanks_WithFromPublicationDateFilter_ShouldReturnMatching()
    {
        var old = PaperBankEntity.Create(Guid.NewGuid(), "Old Paper",
            publicationDate: DateTimeOffset.UtcNow.AddYears(-5));
        var recent = PaperBankEntity.Create(Guid.NewGuid(), "Recent Paper",
            publicationDate: DateTimeOffset.UtcNow.AddMonths(-1));
        Session.Store(old); Session.Store(recent);
        await Session.SaveChangesAsync();

        var handler = new GetPaperBanksQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperBanksQuery(
                new GetPaperBanksFilter { FromPublicationDate = DateTimeOffset.UtcNow.AddYears(-1) },
                new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Title.Should().Be("Recent Paper");
    }

    [Fact]
    public async Task GetPaperBanks_WithToPublicationDateFilter_ShouldReturnMatching()
    {
        var old = PaperBankEntity.Create(Guid.NewGuid(), "Old Paper",
            publicationDate: DateTimeOffset.UtcNow.AddYears(-5));
        var recent = PaperBankEntity.Create(Guid.NewGuid(), "Recent Paper",
            publicationDate: DateTimeOffset.UtcNow.AddMonths(-1));
        Session.Store(old); Session.Store(recent);
        await Session.SaveChangesAsync();

        var handler = new GetPaperBanksQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperBanksQuery(
                new GetPaperBanksFilter { ToPublicationDate = DateTimeOffset.UtcNow.AddYears(-2) },
                new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Title.Should().Be("Old Paper");
    }

    [Fact]
    public async Task GetPaperBanks_WithGapTypeIdFilter_ShouldReturnMatching()
    {
        var gapId = Guid.NewGuid();
        var pb = PaperBankEntity.Create(Guid.NewGuid(), "Gap Paper", gapTypeIds: new List<Guid> { gapId });
        var pb2 = PaperBankEntity.Create(Guid.NewGuid(), "No Gap Paper");
        Session.Store(pb); Session.Store(pb2);
        await Session.SaveChangesAsync();

        var handler = new GetPaperBanksQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperBanksQuery(new GetPaperBanksFilter { GapTypeId = gapId }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPaperBanks_WithJournalIdFilter_ShouldReturnMatching()
    {
        var journalId = Guid.NewGuid();
        var pb = PaperBankEntity.Create(Guid.NewGuid(), "Journal Paper", conferenceJournalId: journalId);
        var pb2 = PaperBankEntity.Create(Guid.NewGuid(), "Other Paper");
        Session.Store(pb); Session.Store(pb2);
        await Session.SaveChangesAsync();

        var handler = new GetPaperBanksQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperBanksQuery(new GetPaperBanksFilter { JournalId = journalId }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPaperBanks_WithRankingFilter_ShouldReturnMatching()
    {
        var pb = PaperBankEntity.Create(Guid.NewGuid(), "Q1 Paper", ranking: "Q1");
        var pb2 = PaperBankEntity.Create(Guid.NewGuid(), "Q2 Paper", ranking: "Q2");
        Session.Store(pb); Session.Store(pb2);
        await Session.SaveChangesAsync();

        var handler = new GetPaperBanksQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperBanksQuery(new GetPaperBanksFilter { Ranking = "Q1" }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPaperBanks_WithKeywordsFilter_ShouldReturnMatching()
    {
        var pb = PaperBankEntity.Create(Guid.NewGuid(), "ML Paper",
            keywords: new List<string> { "machine learning", "neural network" });
        var pb2 = PaperBankEntity.Create(Guid.NewGuid(), "Other Paper",
            keywords: new List<string> { "database", "sql" });
        Session.Store(pb); Session.Store(pb2);
        await Session.SaveChangesAsync();

        var handler = new GetPaperBanksQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperBanksQuery(new GetPaperBanksFilter { Keyword = new[] { "machine learning" } }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPaperBanks_WithExistingPaperIdsFilter_ShouldExcludeThose()
    {
        var pb1 = PaperBankEntity.Create(Guid.NewGuid(), "Existing Paper 1");
        var pb2 = PaperBankEntity.Create(Guid.NewGuid(), "Available Paper");
        Session.Store(pb1); Session.Store(pb2);
        await Session.SaveChangesAsync();

        var handler = new GetPaperBanksQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperBanksQuery(
                new GetPaperBanksFilter { ExistingPaperIds = new[] { pb1.Id } },
                new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Title.Should().Be("Available Paper");
    }

    [Fact]
    public async Task GetPaperBanks_WithJournalEnrichment_ShouldPopulateJournalName()
    {
        var journal = ConferenceJournalEntity.Create(
            Guid.NewGuid(), "IEEE Trans", "Q1", null, null, null,
            ConferenceJournalType.Journal, new List<Guid>(), null, null);
        Session.Store(journal);

        var pb = PaperBankEntity.Create(Guid.NewGuid(), "Journal Paper", conferenceJournalId: journal.Id);
        Session.Store(pb);
        await Session.SaveChangesAsync();

        var handler = new GetPaperBanksQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperBanksQuery(new GetPaperBanksFilter(), new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].ConferenceJournalName.Should().Be("IEEE Trans");
    }

    [Fact]
    public async Task GetPaperBanks_WithGapTypeEnrichment_ShouldPopulateGapTypes()
    {
        var gapType = new GapTypeEntity { Id = Guid.NewGuid(), Name = "Empirical Gap" };
        Session.Store(gapType);

        var pb = PaperBankEntity.Create(Guid.NewGuid(), "Gap Paper",
            gapTypeIds: new List<Guid> { gapType.Id });
        Session.Store(pb);
        await Session.SaveChangesAsync();

        var handler = new GetPaperBanksQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperBanksQuery(new GetPaperBanksFilter(), new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].GapTypes.Should().HaveCount(1);
        result.Items[0].GapTypes[0].Name.Should().Be("Empirical Gap");
    }

    #endregion

    #region GetPaperSamples

    [Fact]
    public async Task GetPaperSamples_WithNoFilter_ShouldReturnAll()
    {
        SeedPaperBank("S1"); SeedPaperBank("S2");
        await Session.SaveChangesAsync();

        var handler = new GetPaperSamplesQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperSamplesQuery(new GetPaperSamplesFilter(), new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPaperSamples_WithTitleFilter_ShouldReturnMatching()
    {
        SeedPaperBank("Targeted Sample"); SeedPaperBank("Other");
        await Session.SaveChangesAsync();

        var handler = new GetPaperSamplesQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperSamplesQuery(new GetPaperSamplesFilter { Title = "Targeted" }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    #endregion
}
