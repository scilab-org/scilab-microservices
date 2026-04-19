using Management.Application.Features.Project.Commands;
using Management.Application.Tests.Common;
using Management.Application.Tests.Common.TestData;

namespace Management.Application.Tests.Features.Project.Handlers;

public class CreateSubProjectCommandHandlerTests : BaseTest
{
    private readonly Mock<IDocumentSession> _sessionMock;
    private readonly CreateSubProjectCommandHandler _handler;

    public CreateSubProjectCommandHandlerTests()
    {
        _sessionMock = new Mock<IDocumentSession>();
        _handler = new CreateSubProjectCommandHandler(_sessionMock.Object);
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var dto = new CreateSubProjectDto { PaperId = Guid.NewGuid(), Name = "Sub Project" };
        var command = new CreateSubProjectCommand(projectId, dto, "testuser");

        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync((ProjectEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken));
    }

    [Fact]
    public async Task Handle_ValidProject_CreatesSubProjectAndReturnsId()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var dto = new CreateSubProjectDto { PaperId = paperId, Name = "Sub Project" };
        var command = new CreateSubProjectCommand(projectId, dto, "testuser");

        var parentProject = ProjectTestData.CreateProjectEntity(id: projectId);
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(parentProject);

        ProjectEntity? storedSubProject = null;
        _sessionMock.Setup(s => s.Store(It.IsAny<ProjectEntity>()))
            .Callback<ProjectEntity[]>(entities =>
            {
                foreach (var entity in entities)
                {
                    if (entity.Id != projectId)
                        storedSubProject = entity;
                }
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().NotBeEmpty();
        _sessionMock.Verify(s => s.Store(It.Is<ProjectEntity>(p =>
            p.ParentProjectId == projectId &&
            p.Name == "Sub Project" &&
            p.PaperIds.Contains(paperId) &&
            p.CreatedBy == "testuser")), Times.Once);
        _sessionMock.Verify(s => s.SaveChangesAsync(CancellationToken), Times.Once);
    }
}
