using Management.Application.Dtos.Papers;
using Management.Application.Features.Project.Queries;
using Management.Application.Models.Results;
using Management.Application.Tests.Common;
using Management.Application.Tests.Common.TestData;

namespace Management.Application.Tests.Features.Project.Handlers;

public class GetProjectPapersQueryHandlerTests : BaseTest
{
    private readonly Mock<IDocumentSession> _sessionMock;
    private readonly Mock<ILabApiService> _labApiServiceMock;
    private readonly GetProjectPapersQueryHandler _handler;

    public GetProjectPapersQueryHandlerTests()
    {
        _sessionMock = new Mock<IDocumentSession>();
        _labApiServiceMock = new Mock<ILabApiService>();
        _handler = new GetProjectPapersQueryHandler(_sessionMock.Object, _labApiServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var query = new GetProjectPapersQuery(projectId, new PaginationRequest(), new GetPaperBanksFilter());

        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync((ProjectEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(query, CancellationToken));
    }

    [Fact]
    public async Task Handle_ProjectWithNoPapers_ReturnsEmptyResult()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var paging = new PaginationRequest { PageNumber = 1, PageSize = 10 };
        var query = new GetProjectPapersQuery(projectId, paging, new GetPaperBanksFilter());

        var project = ProjectTestData.CreateProjectEntity(id: projectId, paperIds: new List<Guid>());
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.Items.Should().BeEmpty();
        _labApiServiceMock.Verify(s => s.GetPaperBanksByIdsPagedAsync(
            It.IsAny<IEnumerable<Guid>>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string[]?>(),
            It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ProjectWithPapers_FetchesPaperDetailsFromLabService()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var paperId1 = Guid.NewGuid();
        var paperId2 = Guid.NewGuid();
        var paging = new PaginationRequest { PageNumber = 1, PageSize = 10 };
        var filter = new GetPaperBanksFilter { Title = "test" };
        var query = new GetProjectPapersQuery(projectId, paging, filter);

        var project = ProjectTestData.CreateProjectEntity(
            id: projectId,
            paperIds: new List<Guid> { paperId1, paperId2, paperId1 }); // includes duplicate
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(project);

        var papers = new List<PaperBankInfoDto>
        {
            new() { Id = paperId1, Title = "Paper 1" },
            new() { Id = paperId2, Title = "Paper 2" }
        };
        _labApiServiceMock.Setup(s => s.GetPaperBanksByIdsPagedAsync(
                It.IsAny<IEnumerable<Guid>>(),
                "test", null, null, null, null, null,
                1, 10, CancellationToken))
            .ReturnsAsync((papers, 2L));

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.Items.Should().HaveCount(2);
        _labApiServiceMock.Verify(s => s.GetPaperBanksByIdsPagedAsync(
            It.Is<IEnumerable<Guid>>(ids => ids.Distinct().Count() == 2),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string[]?>(),
            1, 10, CancellationToken), Times.Once);
    }
}
