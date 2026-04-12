using BuildingBlocks.Pagination;
using User.Application.Features.Users.Queries;
using User.Application.Tests.Common;
using User.Application.Tests.Common.TestData;

namespace User.Application.Tests.Features.Users.Queries;

public sealed class GetUsersQueryHandlerTests : BaseTest
{
    private readonly Mock<IKeycloakService> _keycloakService;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _keycloakService = new Mock<IKeycloakService>();
        _handler = new GetUsersQueryHandler(_keycloakService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedUsers_WhenUsersExist()
    {
        // Arrange
        var users = UserTestData.CreateUserDtoList(count: 5);
        var filter = new GetUsersFilter(SearchText: null);
        var paging = new PaginationRequest(PageNumber: 1, PageSize: 10);
        var query = new GetUsersQuery(filter, paging);

        _keycloakService
            .Setup(s => s.GetUsersAsync(
                filter.SearchText,
                filter.GroupName,
                filter.Enabled,
                paging.PageNumber,
                paging.PageSize,
                null,
                null,
                CancellationToken))
            .ReturnsAsync((users, users.Count));

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(5);
        result.Paging.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoUsersMatch()
    {
        // Arrange
        var filter = new GetUsersFilter(SearchText: "nonexistent");
        var paging = new PaginationRequest(PageNumber: 1, PageSize: 10);
        var query = new GetUsersQuery(filter, paging);

        _keycloakService
            .Setup(s => s.GetUsersAsync(
                filter.SearchText,
                filter.GroupName,
                filter.Enabled,
                paging.PageNumber,
                paging.PageSize,
                null,
                null,
                CancellationToken))
            .ReturnsAsync((new List<UserDto>(), 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldForwardAllFilterParameters_WhenQueryHasFilters()
    {
        // Arrange
        const string excludeUserId = "admin-user-id";
        const string excludeAdminGroup = "super-admins";
        var filter = new GetUsersFilter(SearchText: "john", GroupName: "Researchers", Enabled: true);
        var paging = new PaginationRequest(PageNumber: 2, PageSize: 5);
        var query = new GetUsersQuery(filter, paging, excludeUserId, excludeAdminGroup);

        _keycloakService
            .Setup(s => s.GetUsersAsync(
                "john",
                "Researchers",
                true,
                2,
                5,
                excludeUserId,
                excludeAdminGroup,
                CancellationToken))
            .ReturnsAsync((new List<UserDto>(), 0));

        // Act
        await _handler.Handle(query, CancellationToken);

        // Assert
        _keycloakService.Verify(
            s => s.GetUsersAsync(
                "john",
                "Researchers",
                true,
                2,
                5,
                excludeUserId,
                excludeAdminGroup,
                CancellationToken),
            Times.Once);
    }
}
