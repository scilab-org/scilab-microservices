using Management.Application.Features.Project.Commands;
using Management.Application.Tests.Common;
using Management.Application.Tests.Common.TestData;

namespace Management.Application.Tests.Features.Project.Handlers;

public class CreateProjectConferenceJournalCommandHandlerTests : BaseTest
{
    private readonly Mock<IDocumentSession> _sessionMock;
    private readonly CreateProjectConferenceJournalCommandHandler _handler;

    public CreateProjectConferenceJournalCommandHandlerTests()
    {
        _sessionMock = new Mock<IDocumentSession>();
        _handler = new CreateProjectConferenceJournalCommandHandler(_sessionMock.Object);
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var journalId = Guid.NewGuid();
        var command = new CreateProjectConferenceJournalCommand(projectId, journalId);

        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync((ProjectEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken));
    }

    [Fact]
    public async Task Handle_ValidProject_AddsJournalIdAndReturnsProjectId()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var journalId = Guid.NewGuid();
        var command = new CreateProjectConferenceJournalCommand(projectId, journalId);

        var project = ProjectTestData.CreateProjectEntity(id: projectId, conferenceJournalIds: new List<Guid>());
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().Be(projectId);
        project.ConferenceJournalIds.Should().Contain(journalId);
        _sessionMock.Verify(s => s.Update(project), Times.Once);
        _sessionMock.Verify(s => s.SaveChangesAsync(CancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateJournalId_DeduplicatesAndStoresDistinct()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var journalId = Guid.NewGuid();
        var command = new CreateProjectConferenceJournalCommand(projectId, journalId);

        var project = ProjectTestData.CreateProjectEntity(
            id: projectId,
            conferenceJournalIds: new List<Guid> { journalId });
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().Be(projectId);
        project.ConferenceJournalIds.Count(x => x == journalId).Should().Be(1);
        _sessionMock.Verify(s => s.Update(project), Times.Once);
        _sessionMock.Verify(s => s.SaveChangesAsync(CancellationToken), Times.Once);
    }
}
