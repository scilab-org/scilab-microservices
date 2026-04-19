using Management.Application.Dtos.Papers;
using Management.Application.Features.Project.Queries;
using Management.Application.Models.Results;
using Management.Application.Tests.Common;
using Management.Application.Tests.Common.TestData;

namespace Management.Application.Tests.Features.Project.Handlers;

public class GetAvailablePapersQueryHandlerTests : BaseTest
{
    private readonly Mock<IDocumentSession> _sessionMock;
    private readonly Mock<ILabApiService> _labApiServiceMock;
    private readonly GetAvailablePapersQueryHandler _handler;

    public GetAvailablePapersQueryHandlerTests()
    {
        _sessionMock = new Mock<IDocumentSession>();
        _labApiServiceMock = new Mock<ILabApiService>();
        _handler = new GetAvailablePapersQueryHandler(_sessionMock.Object, _labApiServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var query = new GetAvailablePapersQuery(projectId, new GetPaperBanksFilter(), new PaginationRequest());

        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync((ProjectEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(query, CancellationToken));
    }

    [Fact]
    public async Task Handle_ValidProject_CallsLabServiceAndReturnsResult()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var existingPaperId = Guid.NewGuid();
        var filter = new GetPaperBanksFilter { Title = "test" };
        var paging = new PaginationRequest { PageNumber = 1, PageSize = 10 };
        var query = new GetAvailablePapersQuery(projectId, filter, paging);

        var project = ProjectTestData.CreateProjectEntity(
            id: projectId,
            paperIds: new List<Guid> { existingPaperId });
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(project);

        var papers = new List<PaperBankInfoDto>
        {
            new() { Id = Guid.NewGuid(), Title = "Paper 1" }
        };
        _labApiServiceMock.Setup(s => s.GetAvailablePapersAsync(
                It.IsAny<IEnumerable<Guid>>(),
                "test", null, null, null, null, null, null, null, null, null, null, null,
                1, 10, CancellationToken))
            .ReturnsAsync((papers, 1L));

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        _labApiServiceMock.Verify(s => s.GetAvailablePapersAsync(
            It.IsAny<IEnumerable<Guid>>(),
            It.IsAny<string?>(), It.IsAny<string[]?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int?>(),
            It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string[]?>(), It.IsAny<int>(), It.IsAny<int>(),
            CancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ProjectWithNoPapers_PassesEmptyExistingIds()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var query = new GetAvailablePapersQuery(projectId, new GetPaperBanksFilter(), new PaginationRequest());

        var project = ProjectTestData.CreateProjectEntity(id: projectId, paperIds: new List<Guid>());
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(project);

        _labApiServiceMock.Setup(s => s.GetAvailablePapersAsync(
                It.IsAny<IEnumerable<Guid>>(),
                It.IsAny<string?>(), It.IsAny<string[]?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int?>(),
                It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string[]?>(), It.IsAny<int>(), It.IsAny<int>(),
                CancellationToken))
            .ReturnsAsync((new List<PaperBankInfoDto>(), 0L));

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.Items.Should().BeEmpty();
    }
}
