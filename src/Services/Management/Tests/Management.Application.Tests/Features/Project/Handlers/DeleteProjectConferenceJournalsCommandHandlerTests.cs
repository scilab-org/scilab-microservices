using Management.Application.Features.Project.Commands;
using Management.Application.Tests.Common;
using Management.Application.Tests.Common.TestData;

namespace Management.Application.Tests.Features.Project.Handlers;

public class DeleteProjectConferenceJournalsCommandHandlerTests : BaseTest
{
    private readonly Mock<IDocumentSession> _sessionMock;
    private readonly DeleteProjectConferenceJournalsCommandHandler _handler;

    public DeleteProjectConferenceJournalsCommandHandlerTests()
    {
        _sessionMock = new Mock<IDocumentSession>();
        _handler = new DeleteProjectConferenceJournalsCommandHandler(_sessionMock.Object);
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new DeleteProjectConferenceJournalsCommand(Guid.NewGuid(), Guid.NewGuid());

        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(command.ProjectId, CancellationToken))
            .ReturnsAsync((ProjectEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken));
    }

    [Fact]
    public async Task Handle_ValidProject_RemovesJournalIdAndReturnsProjectId()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var journalId = Guid.NewGuid();
        var command = new DeleteProjectConferenceJournalsCommand(projectId, journalId);

        var project = ProjectTestData.CreateProjectEntity(
            id: projectId,
            conferenceJournalIds: new List<Guid> { journalId, Guid.NewGuid() });
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().Be(projectId);
        project.ConferenceJournalIds.Should().NotContain(journalId);
        _sessionMock.Verify(s => s.Update(project), Times.Once);
        _sessionMock.Verify(s => s.SaveChangesAsync(CancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_JournalIdNotInList_DoesNotThrowAndSaves()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var journalId = Guid.NewGuid();
        var command = new DeleteProjectConferenceJournalsCommand(projectId, journalId);

        var project = ProjectTestData.CreateProjectEntity(
            id: projectId,
            conferenceJournalIds: new List<Guid> { Guid.NewGuid() });
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().Be(projectId);
        _sessionMock.Verify(s => s.Update(project), Times.Once);
        _sessionMock.Verify(s => s.SaveChangesAsync(CancellationToken), Times.Once);
    }
}
