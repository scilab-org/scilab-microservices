using Management.Application.Features.Project.Commands;
using Management.Application.Tests.Common;
using Management.Application.Tests.Common.TestData;

namespace Management.Application.Tests.Features.Project.Handlers;

public class CreateProjectPaperCommandHandlerTests : BaseTest
{
    private readonly Mock<IDocumentSession> _sessionMock;
    private readonly Mock<ILabApiService> _labApiServiceMock;
    private readonly CreateProjectPaperCommandHandler _handler;

    public CreateProjectPaperCommandHandlerTests()
    {
        _sessionMock = new Mock<IDocumentSession>();
        _labApiServiceMock = new Mock<ILabApiService>();
        _handler = new CreateProjectPaperCommandHandler(_sessionMock.Object, _labApiServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var dto = new CreateProjectPaperDto { PaperIds = new List<Guid> { Guid.NewGuid() } };
        var command = new CreateProjectPaperCommand(projectId, dto);

        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync((ProjectEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken));
    }

    [Fact]
    public async Task Handle_NoValidPaperIds_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var paperIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var dto = new CreateProjectPaperDto { PaperIds = paperIds };
        var command = new CreateProjectPaperCommand(projectId, dto);

        var project = ProjectTestData.CreateProjectEntity(id: projectId);
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(project);

        _labApiServiceMock.Setup(s => s.GetExistingPaperBankIdsAsync(paperIds, CancellationToken))
            .ReturnsAsync(new List<Guid>());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken));
    }

    [Fact]
    public async Task Handle_ValidPaperIds_AddsPapersAndReturnsIds()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var paperId1 = Guid.NewGuid();
        var paperId2 = Guid.NewGuid();
        var paperIds = new List<Guid> { paperId1, paperId2 };
        var dto = new CreateProjectPaperDto { PaperIds = paperIds };
        var command = new CreateProjectPaperCommand(projectId, dto);

        var project = ProjectTestData.CreateProjectEntity(id: projectId, paperIds: new List<Guid>());
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(project);

        var validIds = new List<Guid> { paperId1 };
        _labApiServiceMock.Setup(s => s.GetExistingPaperBankIdsAsync(paperIds, CancellationToken))
            .ReturnsAsync(validIds);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().BeEquivalentTo(validIds);
        project.PaperIds.Should().Contain(paperId1);
        _sessionMock.Verify(s => s.Store(project), Times.Once);
        _sessionMock.Verify(s => s.SaveChangesAsync(CancellationToken), Times.Once);
    }
}
