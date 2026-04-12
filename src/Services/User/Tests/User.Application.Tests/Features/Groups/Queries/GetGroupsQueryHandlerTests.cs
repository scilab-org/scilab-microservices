using User.Application.Features.Groups.Queries;
using User.Application.Tests.Common;
using User.Application.Tests.Common.TestData;

namespace User.Application.Tests.Features.Groups.Queries;

public sealed class GetGroupsQueryHandlerTests : BaseTest
{
    private readonly Mock<IKeycloakService> _keycloakService;
    private readonly GetGroupsQueryHandler _handler;

    public GetGroupsQueryHandlerTests()
    {
        _keycloakService = new Mock<IKeycloakService>();
        _handler = new GetGroupsQueryHandler(_keycloakService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllGroups_WhenGroupsExist()
    {
        // Arrange
        var groups = GroupTestData.CreateGroupDtoList(count: 4);
        var query = new GetGroupsQuery();

        _keycloakService
            .Setup(s => s.GetGroupsAsync(CancellationToken))
            .ReturnsAsync(groups);

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4);
        result.Should().AllSatisfy(g => g.Id.Should().NotBeNullOrEmpty());
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoGroupsExist()
    {
        // Arrange
        var query = new GetGroupsQuery();

        _keycloakService
            .Setup(s => s.GetGroupsAsync(CancellationToken))
            .ReturnsAsync(new List<GroupDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldCallGetGroupsAsyncOnce()
    {
        // Arrange
        var query = new GetGroupsQuery();

        _keycloakService
            .Setup(s => s.GetGroupsAsync(CancellationToken))
            .ReturnsAsync(new List<GroupDto>());

        // Act
        await _handler.Handle(query, CancellationToken);

        // Assert
        _keycloakService.Verify(s => s.GetGroupsAsync(CancellationToken), Times.Once);
    }
}
