using Management.Application.Features.Project.Commands;
using Management.Application.Tests.Common;
using Management.Application.Tests.Common.TestData;

namespace Management.Application.Tests.Features.Project.Handlers;

public class DeleteProjectPapersCommandHandlerTests : BaseTest
{
    private readonly Mock<IDocumentSession> _sessionMock;
    private readonly DeleteProjectPapersCommandHandler _handler;

    public DeleteProjectPapersCommandHandlerTests()
    {
        _sessionMock = new Mock<IDocumentSession>();
        _handler = new DeleteProjectPapersCommandHandler(_sessionMock.Object);
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var dto = new DeleteProjectPaperDto { PaperIds = new List<Guid> { Guid.NewGuid() } };
        var command = new DeleteProjectPapersCommand(projectId, dto);

        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync((ProjectEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken));
    }

    [Fact]
    public async Task Handle_AllPaperIdsEmpty_ThrowsClientValidationException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var dto = new DeleteProjectPaperDto { PaperIds = new List<Guid> { Guid.Empty, Guid.Empty } };
        var command = new DeleteProjectPapersCommand(projectId, dto);

        var project = ProjectTestData.CreateProjectEntity(id: projectId);
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(project);

        // Act & Assert
        await Assert.ThrowsAsync<ClientValidationException>(
            () => _handler.Handle(command, CancellationToken));
    }

    [Fact]
    public async Task Handle_PapersNotFoundInProject_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var paperIdToRemove = Guid.NewGuid();
        var dto = new DeleteProjectPaperDto { PaperIds = new List<Guid> { paperIdToRemove } };
        var command = new DeleteProjectPapersCommand(projectId, dto);

        var project = ProjectTestData.CreateProjectEntity(
            id: projectId,
            paperIds: new List<Guid> { Guid.NewGuid() }); // different paper
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(project);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken));
    }

    [Fact]
    public async Task Handle_ValidPaperIds_RemovesPapersAndReturnsRemovedIds()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var paperId1 = Guid.NewGuid();
        var paperId2 = Guid.NewGuid();
        var paperIdToKeep = Guid.NewGuid();
        var dto = new DeleteProjectPaperDto { PaperIds = new List<Guid> { paperId1, paperId2 } };
        var command = new DeleteProjectPapersCommand(projectId, dto);

        var project = ProjectTestData.CreateProjectEntity(
            id: projectId,
            paperIds: new List<Guid> { paperId1, paperId2, paperIdToKeep });
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().Contain(paperId1);
        result.Should().Contain(paperId2);
        project.PaperIds.Should().Contain(paperIdToKeep);
        project.PaperIds.Should().NotContain(paperId1);
        project.PaperIds.Should().NotContain(paperId2);
        _sessionMock.Verify(s => s.Store(project), Times.Once);
        _sessionMock.Verify(s => s.SaveChangesAsync(CancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicatePaperIds_DeduplicatesBeforeRemoval()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var dto = new DeleteProjectPaperDto { PaperIds = new List<Guid> { paperId, paperId } };
        var command = new DeleteProjectPapersCommand(projectId, dto);

        var project = ProjectTestData.CreateProjectEntity(
            id: projectId,
            paperIds: new List<Guid> { paperId });
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().Contain(paperId);
        _sessionMock.Verify(s => s.Store(project), Times.Once);
        _sessionMock.Verify(s => s.SaveChangesAsync(CancellationToken), Times.Once);
    }
}
