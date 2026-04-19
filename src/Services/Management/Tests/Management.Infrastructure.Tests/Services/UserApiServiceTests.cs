using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Management.Infrastructure.ApiClients;
using Management.Infrastructure.Services;
using Management.Infrastructure.Exceptions;

namespace Management.Infrastructure.Tests.Services;

public sealed class UserApiServiceTests
{
    private readonly Mock<IUserServiceApi> _apiMock = new();
    private readonly UserApiService _sut;

    public UserApiServiceTests()
    {
        _sut = new UserApiService(_apiMock.Object);
    }

    // ==========================================
    // GetExistingUserIdsAsync
    // ==========================================

    [Fact]
    public async Task GetExistingUserIdsAsync_Should_ReturnValidIds()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        _apiMock.Setup(a => a.GetUserByIdAsync(id1.ToString()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
        _apiMock.Setup(a => a.GetUserByIdAsync(id2.ToString()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var result = await _sut.GetExistingUserIdsAsync(new[] { id1, id2 });

        result.Should().ContainSingle().Which.Should().Be(id1);
    }

    [Fact]
    public async Task GetExistingUserIdsAsync_Should_SkipOnException()
    {
        var id = Guid.NewGuid();
        _apiMock.Setup(a => a.GetUserByIdAsync(id.ToString()))
            .ThrowsAsync(new Exception("unreachable"));

        var result = await _sut.GetExistingUserIdsAsync(new[] { id });

        result.Should().BeEmpty();
    }

    // ==========================================
    // IsUserExistAsync
    // ==========================================

    [Fact]
    public async Task IsUserExistAsync_Should_ReturnTrue_WhenUserExists()
    {
        var id = Guid.NewGuid();
        _apiMock.Setup(a => a.GetUserByIdAsync(id.ToString()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var result = await _sut.IsUserExistAsync(id);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsUserExistAsync_Should_ReturnFalse_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _apiMock.Setup(a => a.GetUserByIdAsync(id.ToString()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var result = await _sut.IsUserExistAsync(id);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsUserExistAsync_Should_ReturnFalse_WhenException()
    {
        var id = Guid.NewGuid();
        _apiMock.Setup(a => a.GetUserByIdAsync(id.ToString()))
            .ThrowsAsync(new Exception("err"));

        var result = await _sut.IsUserExistAsync(id);

        result.Should().BeFalse();
    }

    // ==========================================
    // GetAvailableProjectUsersAsync
    // ==========================================

    [Fact]
    public async Task GetAvailableProjectUsersAsync_Should_ReturnEmptyList_WhenApiFails()
    {
        _apiMock.Setup(a => a.GetUsersAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool?>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var result = await _sut.GetAvailableProjectUsersAsync(Array.Empty<Guid>(), "admin");

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAvailableProjectUsersAsync_Should_FilterExistingMembers()
    {
        var memberId = Guid.NewGuid();
        var body = new
        {
            Result = new
            {
                Items = new[]
                {
                    new { Id = memberId.ToString(), Username = "existing", Email = "e@e.com", FirstName = "A", LastName = "B", Enabled = true, Groups = (object[]?)null },
                    new { Id = Guid.NewGuid().ToString(), Username = "available", Email = "a@a.com", FirstName = "C", LastName = "D", Enabled = true, Groups = (object[]?)null }
                }
            }
        };
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(body)
        };
        _apiMock.Setup(a => a.GetUsersAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool?>())).ReturnsAsync(response);

        var result = await _sut.GetAvailableProjectUsersAsync(new[] { memberId }, "admin");

        result.Should().HaveCount(1);
        result[0].Username.Should().Be("available");
    }

    // ==========================================
    // GetUsersByIdsAsync
    // ==========================================

    [Fact]
    public async Task GetUsersByIdsAsync_Should_ReturnEmptyList_WhenNoIds()
    {
        var result = await _sut.GetUsersByIdsAsync(Array.Empty<Guid>());

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUsersByIdsAsync_Should_SkipFailedRequests()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var body = new { Result = new { User = new { Id = id1.ToString(), Username = "u1", Email = "e@e", FirstName = "F", LastName = "L", Enabled = true, Groups = new[] { new { Name = "group1" } } } } };
        _apiMock.Setup(a => a.GetUserByIdAsync(id1.ToString()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(body) });
        _apiMock.Setup(a => a.GetUserByIdAsync(id2.ToString()))
            .ThrowsAsync(new Exception("err"));

        var result = await _sut.GetUsersByIdsAsync(new[] { id1, id2 });

        result.Should().HaveCount(1);
    }

    // ==========================================
    // AssignUserRoleAsync
    // ==========================================

    [Fact]
    public async Task AssignUserRoleAsync_Should_ThrowInfrastructureException_WhenUserNotFound()
    {
        var id = Guid.NewGuid();
        _apiMock.Setup(a => a.GetUserByIdAsync(id.ToString()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var act = () => _sut.AssignUserRoleAsync(id, "role");

        await act.Should().ThrowAsync<InfrastructureException>();
    }

    [Fact]
    public async Task AssignUserRoleAsync_Should_AddRole_WhenUserExists()
    {
        var id = Guid.NewGuid();
        var body = new { Result = new { User = new { Id = id.ToString(), Username = "u", Email = "e@e", FirstName = "F", LastName = "L", Enabled = true, Groups = new[] { new { Name = "existing-group" } } } } };
        _apiMock.Setup(a => a.GetUserByIdAsync(id.ToString()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(body) });
        _apiMock.Setup(a => a.UpdateUserGroupsAsync(id.ToString(), It.IsAny<UpdateUserGroupsRequest>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        await _sut.AssignUserRoleAsync(id, "new-role");

        _apiMock.Verify(a => a.UpdateUserGroupsAsync(id.ToString(),
            It.Is<UpdateUserGroupsRequest>(r => r.GroupNames.Contains("new-role") && r.GroupNames.Contains("existing-group"))),
            Times.Once);
    }

    [Fact]
    public async Task AssignUserRoleAsync_Should_ThrowInfrastructureException_WhenUpdateFails()
    {
        var id = Guid.NewGuid();
        var body = new { Result = new { User = new { Id = id.ToString(), Username = "u", Email = "e@e", FirstName = "F", LastName = "L", Enabled = true, Groups = (object[]?)null } } };
        _apiMock.Setup(a => a.GetUserByIdAsync(id.ToString()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(body) });
        _apiMock.Setup(a => a.UpdateUserGroupsAsync(id.ToString(), It.IsAny<UpdateUserGroupsRequest>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var act = () => _sut.AssignUserRoleAsync(id, "role");

        await act.Should().ThrowAsync<InfrastructureException>();
    }
}
