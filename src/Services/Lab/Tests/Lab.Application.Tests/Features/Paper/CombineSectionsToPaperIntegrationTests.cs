using Common.Constants;
using Lab.Application.Dtos.Papers;
using Lab.Application.Features.Paper.Commands.CombineSectionsToPaper;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;
using Lab.Domain.Enums;

namespace Lab.Application.Tests.Features.Paper;

public class CombineSectionsToPaperIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "combine_sections_tests";

    private readonly Mock<IManagementApiService> _mockMgmt = new();
    private readonly Mock<IAiApiService> _mockAi = new();
    private readonly Mock<IHttpClientFactory> _mockHttpFactory = new();

    private (PaperEntity paper, ConferenceJournalEntity journal) SeedPaperWithJournal(string title = "Test Paper")
    {
        var journal = ConferenceJournalEntity.Create(
            Guid.NewGuid(), "Test Journal", "Q1", null, null, null,
            ConferenceJournalType.Journal, new List<Guid>(), null, null);
        var paper = PaperEntity.Create(Guid.NewGuid(), title, conferenceJournalId: journal.Id);
        Session.Store(journal);
        Session.Store(paper);
        return (paper, journal);
    }

    // ─── Auth failures ────────────────────────────────────────────────────────

    [Fact]
    public async Task CombineSectionsToPaper_WhenRoleIsNull_ShouldThrowUnauthorizedException()
    {
        var projectId = Guid.NewGuid();
        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var handler = new CombineSectionsToPaperCommandHandler(
            Session, _mockMgmt.Object, _mockAi.Object, _mockHttpFactory.Object);
        var dto = new CreatePaperCombineDto { ProjectId = projectId };

        var act = () => handler.Handle(
            new CombineSectionsToPaperCommand(Guid.NewGuid(), dto, "user"),
            CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task CombineSectionsToPaper_WhenRoleIsNotPaperAuthor_ShouldThrowUnauthorizedException()
    {
        var projectId = Guid.NewGuid();
        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperMember);

        var handler = new CombineSectionsToPaperCommandHandler(
            Session, _mockMgmt.Object, _mockAi.Object, _mockHttpFactory.Object);
        var dto = new CreatePaperCombineDto { ProjectId = projectId };

        var act = () => handler.Handle(
            new CombineSectionsToPaperCommand(Guid.NewGuid(), dto, "user"),
            CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task CombineSectionsToPaper_WhenRoleIsEmpty_ShouldThrowUnauthorizedException()
    {
        var projectId = Guid.NewGuid();
        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(string.Empty);

        var handler = new CombineSectionsToPaperCommandHandler(
            Session, _mockMgmt.Object, _mockAi.Object, _mockHttpFactory.Object);
        var dto = new CreatePaperCombineDto { ProjectId = projectId };

        var act = () => handler.Handle(
            new CombineSectionsToPaperCommand(Guid.NewGuid(), dto, "user"),
            CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    // ─── Paper not found ──────────────────────────────────────────────────────

    [Fact]
    public async Task CombineSectionsToPaper_WithNonExistentPaper_ShouldThrowNotFoundException()
    {
        var projectId = Guid.NewGuid();
        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);

        var handler = new CombineSectionsToPaperCommandHandler(
            Session, _mockMgmt.Object, _mockAi.Object, _mockHttpFactory.Object);
        var dto = new CreatePaperCombineDto { ProjectId = projectId };

        var act = () => handler.Handle(
            new CombineSectionsToPaperCommand(Guid.NewGuid(), dto, "user"),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
