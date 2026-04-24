using Microsoft.Extensions.Configuration;
using Refit;
using Common.Constants;
using User.Application.Dtos.Groups;
using User.Application.Dtos.Roles;
using User.Application.Dtos.Users;
using Common.Constants;

namespace User.Infrastructure.Tests.Services;

public sealed class KeycloakServiceTests
{
    #region Setup

    private readonly Mock<IKeycloakApi> _keycloakApiMock = new();
    private readonly KeycloakService _sut;

    private const string Realm = "test-realm";
    private const string AccessToken = "test-access-token";

    public KeycloakServiceTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{ApiClientCfg.Keycloak.Section}:{ApiClientCfg.Keycloak.Realm}"] = Realm,
                [$"{ApiClientCfg.Keycloak.Section}:{ApiClientCfg.Keycloak.ClientId}"] = "svc-client",
                [$"{ApiClientCfg.Keycloak.Section}:{ApiClientCfg.Keycloak.ClientSecret}"] = "secret",
                [$"{ApiClientCfg.Keycloak.Section}:{ApiClientCfg.Keycloak.GrantType}"] = "client_credentials",
                [$"{ApiClientCfg.Keycloak.Section}:{ApiClientCfg.Keycloak.Scopes}:0"] = "openid",
            })
            .Build();

        _sut = new KeycloakService(
            _keycloakApiMock.Object,
            config,
            NullLogger<KeycloakService>.Instance);
    }

    private void SetupGetAccessToken()
    {
        _keycloakApiMock
            .Setup(x => x.GetAccessTokenAsync(Realm, It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(new KeycloakAccessTokenResponse { AccessToken = AccessToken });
    }

    private static async Task<ApiException> CreateApiException(HttpStatusCode statusCode)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "http://keycloak.test");
        var response = new HttpResponseMessage(statusCode);
        return await ApiException.Create(request, HttpMethod.Get, response, new RefitSettings());
    }

    #endregion

    #region GetAccessTokenAsync (via CreateUserAsync path)

    [Fact]
    public async Task AnyMethod_ShouldThrowInfrastructureException_WhenTokenRetrievalFails()
    {
        // Arrange
        _keycloakApiMock
            .Setup(x => x.GetAccessTokenAsync(Realm, It.IsAny<Dictionary<string, string>>()))
            .ThrowsAsync(new HttpRequestException("network error"));

        // Act
        var act = () => _sut.GetGroupsAsync(CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.FailedToGetAccessToken);
    }

    [Fact]
    public async Task AnyMethod_ShouldThrowInfrastructureException_WhenTokenIsEmpty()
    {
        // Arrange
        _keycloakApiMock
            .Setup(x => x.GetAccessTokenAsync(Realm, It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(new KeycloakAccessTokenResponse { AccessToken = string.Empty });

        // Act
        var act = () => _sut.GetGroupsAsync(CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.FailedToGetAccessToken);
    }

    #endregion

    #region GetGroupsAsync

    [Fact]
    public async Task GetGroupsAsync_ShouldReturnMappedGroups()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ReturnsAsync([
                new KeycloakGroupResponse { Id = "g1", Name = "Admins", Path = "/Admins" },
                new KeycloakGroupResponse { Id = "g2", Name = "Users", Path = "/Users" }
            ]);

        // Act
        var result = await _sut.GetGroupsAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Id.Should().Be("g1");
        result[0].Name.Should().Be("Admins");
        result[1].Id.Should().Be("g2");
    }

    [Fact]
    public async Task GetGroupsAsync_ShouldThrowInfrastructureException_WhenApiCallFails()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ThrowsAsync(new HttpRequestException("server error"));

        // Act
        var act = () => _sut.GetGroupsAsync(CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.FailedToGetGroups);
    }

    #endregion

    #region GetRealmRolesAsync

    [Fact]
    public async Task GetRealmRolesAsync_ShouldReturnMappedRoles()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetRealmRolesAsync(Realm, AccessToken))
            .ReturnsAsync([
                new KeycloakRoleResponse { Id = "r1", Name = "admin", Description = "Administrator" },
                new KeycloakRoleResponse { Id = "r2", Name = "user", Description = "Regular user" }
            ]);

        // Act
        var result = await _sut.GetRealmRolesAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Id.Should().Be("r1");
        result[0].Name.Should().Be("admin");
    }

    [Fact]
    public async Task GetRealmRolesAsync_ShouldThrowInfrastructureException_WhenApiCallFails()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetRealmRolesAsync(Realm, AccessToken))
            .ThrowsAsync(new HttpRequestException("server error"));

        // Act
        var act = () => _sut.GetRealmRolesAsync(CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.FailedToGetRoles);
    }

    #endregion

    #region GetGroupRolesAsync

    [Fact]
    public async Task GetGroupRolesAsync_ShouldReturnMappedRoles()
    {
        // Arrange
        const string groupId = "group-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetGroupRealmRolesAsync(Realm, groupId, AccessToken))
            .ReturnsAsync([
                new KeycloakRoleResponse { Id = "r1", Name = "editor" }
            ]);

        // Act
        var result = await _sut.GetGroupRolesAsync(groupId, CancellationToken.None);

        // Assert
        result.Should().ContainSingle();
        result[0].Name.Should().Be("editor");
    }

    [Fact]
    public async Task GetGroupRolesAsync_ShouldThrowInfrastructureException_WithGroupNotFound_WhenApiReturns404()
    {
        // Arrange
        const string groupId = "missing-group";
        SetupGetAccessToken();
        var apiEx = await CreateApiException(HttpStatusCode.NotFound);
        _keycloakApiMock
            .Setup(x => x.GetGroupRealmRolesAsync(Realm, groupId, AccessToken))
            .ThrowsAsync(apiEx);

        // Act
        var act = () => _sut.GetGroupRolesAsync(groupId, CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.GroupNotFound);
    }

    [Fact]
    public async Task GetGroupRolesAsync_ShouldThrowInfrastructureException_WhenApiCallFails()
    {
        // Arrange
        const string groupId = "group-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetGroupRealmRolesAsync(Realm, groupId, AccessToken))
            .ThrowsAsync(new HttpRequestException("server error"));

        // Act
        var act = () => _sut.GetGroupRolesAsync(groupId, CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.FailedToGetGroupRoles);
    }

    #endregion

    #region GetUserByIdAsync

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnMappedUser()
    {
        // Arrange
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetUserByIdAsync(Realm, userId, AccessToken))
            .ReturnsAsync(new KeycloakUserResponse
            {
                Id = userId,
                Username = "johndoe",
                Email = "john@example.com",
                Enabled = true
            });
        _keycloakApiMock
            .Setup(x => x.GetUserGroupsAsync(Realm, userId, AccessToken))
            .ReturnsAsync([]);
        _keycloakApiMock
            .Setup(x => x.GetUserRealmRoleMappingsAsync(Realm, userId, AccessToken))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.GetUserByIdAsync(userId, CancellationToken.None);

        // Assert
        result.Id.Should().Be(userId);
        result.Username.Should().Be("johndoe");
        result.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldThrowInfrastructureException_WithUserNotFound_WhenApiReturns404()
    {
        // Arrange
        const string userId = "missing-user";
        SetupGetAccessToken();
        var apiEx = await CreateApiException(HttpStatusCode.NotFound);
        _keycloakApiMock
            .Setup(x => x.GetUserByIdAsync(Realm, userId, AccessToken))
            .ThrowsAsync(apiEx);

        // Act
        var act = () => _sut.GetUserByIdAsync(userId, CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.UserNotFound);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldThrowInfrastructureException_WhenApiCallFails()
    {
        // Arrange
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetUserByIdAsync(Realm, userId, AccessToken))
            .ThrowsAsync(new HttpRequestException("server error"));

        // Act
        var act = () => _sut.GetUserByIdAsync(userId, CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.FailedToRetrieveUser);
    }

    #endregion

    #region DeactivateUserAsync

    [Fact]
    public async Task DeactivateUserAsync_ShouldCallUpdateUser_WithEnabledFalse()
    {
        // Arrange
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.UpdateUserAsync(
                Realm,
                userId,
                It.Is<KeycloakUpdateUserRequest>(r => r.Enabled == false),
                AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.DeactivateUserAsync(userId, CancellationToken.None);

        // Assert
        _keycloakApiMock.Verify(
            x => x.UpdateUserAsync(
                Realm,
                userId,
                It.Is<KeycloakUpdateUserRequest>(r => r.Enabled == false),
                AccessToken),
            Times.Once);
    }

    [Fact]
    public async Task DeactivateUserAsync_ShouldThrowInfrastructureException_WithUserNotFound_WhenApiReturns404()
    {
        // Arrange
        const string userId = "missing-user";
        SetupGetAccessToken();
        var apiEx = await CreateApiException(HttpStatusCode.NotFound);
        _keycloakApiMock
            .Setup(x => x.UpdateUserAsync(Realm, userId, It.IsAny<KeycloakUpdateUserRequest>(), AccessToken))
            .ThrowsAsync(apiEx);

        // Act
        var act = () => _sut.DeactivateUserAsync(userId, CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.UserNotFound);
    }

    [Fact]
    public async Task DeactivateUserAsync_ShouldThrowInfrastructureException_WhenApiCallFails()
    {
        // Arrange
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.UpdateUserAsync(Realm, userId, It.IsAny<KeycloakUpdateUserRequest>(), AccessToken))
            .ThrowsAsync(new HttpRequestException("network error"));

        // Act
        var act = () => _sut.DeactivateUserAsync(userId, CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.FailedToDeactivateUser);
    }

    #endregion

    #region ActivateUserAsync

    [Fact]
    public async Task ActivateUserAsync_ShouldCallUpdateUser_WithEnabledTrue()
    {
        // Arrange
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.UpdateUserAsync(
                Realm,
                userId,
                It.Is<KeycloakUpdateUserRequest>(r => r.Enabled == true),
                AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.ActivateUserAsync(userId, CancellationToken.None);

        // Assert
        _keycloakApiMock.Verify(
            x => x.UpdateUserAsync(
                Realm,
                userId,
                It.Is<KeycloakUpdateUserRequest>(r => r.Enabled == true),
                AccessToken),
            Times.Once);
    }

    [Fact]
    public async Task ActivateUserAsync_ShouldThrowInfrastructureException_WithUserNotFound_WhenApiReturns404()
    {
        // Arrange
        const string userId = "missing";
        SetupGetAccessToken();
        var apiEx = await CreateApiException(HttpStatusCode.NotFound);
        _keycloakApiMock
            .Setup(x => x.UpdateUserAsync(Realm, userId, It.IsAny<KeycloakUpdateUserRequest>(), AccessToken))
            .ThrowsAsync(apiEx);

        // Act
        var act = () => _sut.ActivateUserAsync(userId, CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.UserNotFound);
    }

    [Fact]
    public async Task ActivateUserAsync_ShouldThrowInfrastructureException_WhenApiCallFails()
    {
        // Arrange
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.UpdateUserAsync(Realm, userId, It.IsAny<KeycloakUpdateUserRequest>(), AccessToken))
            .ThrowsAsync(new HttpRequestException("network error"));

        // Act
        var act = () => _sut.ActivateUserAsync(userId, CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.FailedToActivateUser);
    }

    #endregion

    #region AddRolesToGroupAsync

    [Fact]
    public async Task AddRolesToGroupAsync_ShouldCallAddOnApi_WhenRolesExist()
    {
        // Arrange
        const string groupId = "group-123";
        var roleNames = new List<string> { "admin" };
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetRealmRolesAsync(Realm, AccessToken))
            .ReturnsAsync([new KeycloakRoleResponse { Id = "r1", Name = "admin" }]);
        _keycloakApiMock
            .Setup(x => x.AddRolesToGroupAsync(Realm, groupId, It.IsAny<List<KeycloakRoleResponse>>(), AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.AddRolesToGroupAsync(groupId, roleNames, CancellationToken.None);

        // Assert
        _keycloakApiMock.Verify(
            x => x.AddRolesToGroupAsync(Realm, groupId, It.IsAny<List<KeycloakRoleResponse>>(), AccessToken),
            Times.Once);
    }

    [Fact]
    public async Task AddRolesToGroupAsync_ShouldThrowInfrastructureException_WhenRoleNameNotFound()
    {
        // Arrange
        const string groupId = "group-123";
        var roleNames = new List<string> { "nonexistent-role" };
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetRealmRolesAsync(Realm, AccessToken))
            .ReturnsAsync([new KeycloakRoleResponse { Id = "r1", Name = "admin" }]);

        // Act
        var act = () => _sut.AddRolesToGroupAsync(groupId, roleNames, CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.RoleNotFound);
    }

    [Fact]
    public async Task AddRolesToGroupAsync_ShouldThrowInfrastructureException_WithGroupNotFound_WhenApiReturns404()
    {
        // Arrange
        const string groupId = "missing-group";
        var roleNames = new List<string> { "admin" };
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetRealmRolesAsync(Realm, AccessToken))
            .ReturnsAsync([new KeycloakRoleResponse { Id = "r1", Name = "admin" }]);
        var apiEx = await CreateApiException(HttpStatusCode.NotFound);
        _keycloakApiMock
            .Setup(x => x.AddRolesToGroupAsync(Realm, groupId, It.IsAny<List<KeycloakRoleResponse>>(), AccessToken))
            .ThrowsAsync(apiEx);

        // Act
        var act = () => _sut.AddRolesToGroupAsync(groupId, roleNames, CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.GroupNotFound);
    }

    #endregion

    #region RemoveRolesFromGroupAsync

    [Fact]
    public async Task RemoveRolesFromGroupAsync_ShouldCallRemoveOnApi_WhenRolesExist()
    {
        // Arrange
        const string groupId = "group-123";
        var roleNames = new List<string> { "admin" };
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetRealmRolesAsync(Realm, AccessToken))
            .ReturnsAsync([new KeycloakRoleResponse { Id = "r1", Name = "admin" }]);
        _keycloakApiMock
            .Setup(x => x.RemoveRolesFromGroupAsync(Realm, groupId, It.IsAny<List<KeycloakRoleResponse>>(), AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.RemoveRolesFromGroupAsync(groupId, roleNames, CancellationToken.None);

        // Assert
        _keycloakApiMock.Verify(
            x => x.RemoveRolesFromGroupAsync(Realm, groupId, It.IsAny<List<KeycloakRoleResponse>>(), AccessToken),
            Times.Once);
    }

    [Fact]
    public async Task RemoveRolesFromGroupAsync_ShouldThrowInfrastructureException_WithGroupNotFound_WhenApiReturns404()
    {
        // Arrange
        const string groupId = "missing-group";
        var roleNames = new List<string> { "admin" };
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetRealmRolesAsync(Realm, AccessToken))
            .ReturnsAsync([new KeycloakRoleResponse { Id = "r1", Name = "admin" }]);
        var apiEx = await CreateApiException(HttpStatusCode.NotFound);
        _keycloakApiMock
            .Setup(x => x.RemoveRolesFromGroupAsync(Realm, groupId, It.IsAny<List<KeycloakRoleResponse>>(), AccessToken))
            .ThrowsAsync(apiEx);

        // Act
        var act = () => _sut.RemoveRolesFromGroupAsync(groupId, roleNames, CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.GroupNotFound);
    }

    #endregion

    #region CreateUserAsync

    [Fact]
    public async Task CreateUserAsync_ShouldReturnUserId_OnSuccess()
    {
        // Arrange
        const string userId = "new-user-id";
        SetupGetAccessToken();

        var createResponse = new HttpResponseMessage(HttpStatusCode.Created);
        createResponse.Headers.Location = new Uri($"http://keycloak/users/{userId}");
        _keycloakApiMock
            .Setup(x => x.CreateUserAsync(Realm, It.IsAny<KeycloakCreateUserRequest>(), AccessToken))
            .ReturnsAsync(createResponse);

        _keycloakApiMock
            .Setup(x => x.GetUsersAsync(Realm, "johndoe", true, AccessToken))
            .ReturnsAsync([new KeycloakUserResponse { Id = userId, Username = "johndoe" }]);

        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.CreateUserAsync(
            username: "johndoe",
            email: "john@example.com",
            firstName: "John",
            lastName: "Doe",
            ocrId: null,
            initialPassword: "Pass123!",
            groupNames: [],
            cancellationToken: CancellationToken.None);

        // Assert
        result.Should().Be(userId);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowInfrastructureException_WithUserAlreadyExists_WhenApiReturns409()
    {
        // Arrange
        SetupGetAccessToken();
        var conflictResponse = new HttpResponseMessage(HttpStatusCode.Conflict);
        _keycloakApiMock
            .Setup(x => x.CreateUserAsync(Realm, It.IsAny<KeycloakCreateUserRequest>(), AccessToken))
            .ReturnsAsync(conflictResponse);

        // Act
        var act = () => _sut.CreateUserAsync(
            "existing-user", "e@e.com", null, null, null, "pass",
            cancellationToken: CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.UserAlreadyExists);
    }

    #endregion

    #region GetUsersAsync

    [Fact]
    public async Task GetUsersAsync_ShouldReturnUsersWithTotalCount_WhenNoGroupFilter()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetUsersCountAsync(Realm, null, AccessToken, null))
            .ReturnsAsync(2);
        _keycloakApiMock
            .Setup(x => x.SearchUsersAsync(Realm, null, 0, 10, AccessToken, null, false))
            .ReturnsAsync([
                new KeycloakUserResponse { Id = "u1", Username = "alice" },
                new KeycloakUserResponse { Id = "u2", Username = "bob" }
            ]);
        _keycloakApiMock
            .Setup(x => x.GetUserGroupsAsync(Realm, It.IsAny<string>(), AccessToken))
            .ReturnsAsync([]);

        // Act
        var (users, total) = await _sut.GetUsersAsync(
            searchText: null,
            groupName: null,
            enabled: null,
            pageNumber: 1,
            pageSize: 10,
            cancellationToken: CancellationToken.None);

        // Assert
        users.Should().HaveCount(2);
        total.Should().Be(2);
    }

    [Fact]
    public async Task GetUsersAsync_ShouldReturnEmpty_WhenGroupNotFoundByName()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, "NonExistentGroup", AccessToken))
            .ReturnsAsync([]);

        // Act
        var (users, total) = await _sut.GetUsersAsync(
            searchText: null,
            groupName: "NonExistentGroup",
            enabled: null,
            pageNumber: 1,
            pageSize: 10,
            cancellationToken: CancellationToken.None);

        // Assert
        users.Should().BeEmpty();
        total.Should().Be(0);
    }

    [Fact]
    public async Task GetUsersAsync_ShouldReturnFilteredUsers_WhenGroupFilterProvided()
    {
        // Arrange
        SetupGetAccessToken();
        var group = new KeycloakGroupResponse { Id = "g1", Name = "Developers", Path = "/Developers" };
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, "Developers", AccessToken))
            .ReturnsAsync([group]);
        _keycloakApiMock
            .Setup(x => x.GetGroupMembersAsync(Realm, "g1", 0, 10, AccessToken, false))
            .ReturnsAsync([new KeycloakUserResponse { Id = "u1", Username = "alice" }]);
        _keycloakApiMock
            .Setup(x => x.GetGroupMembersAsync(Realm, "g1", 0, 10_000, AccessToken, true))
            .ReturnsAsync([new KeycloakUserResponse { Id = "u1" }]);
        _keycloakApiMock
            .Setup(x => x.GetUserGroupsAsync(Realm, "u1", AccessToken))
            .ReturnsAsync([group]);

        // Act
        var (users, total) = await _sut.GetUsersAsync(
            searchText: null,
            groupName: "Developers",
            enabled: null,
            pageNumber: 1,
            pageSize: 10,
            cancellationToken: CancellationToken.None);

        // Assert
        users.Should().ContainSingle();
        users[0].Username.Should().Be("alice");
        total.Should().Be(1);
    }

    [Fact]
    public async Task GetUsersAsync_ShouldExcludeUser_WhenExcludeUserIdProvided()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetUsersCountAsync(Realm, null, AccessToken, null))
            .ReturnsAsync(2);
        _keycloakApiMock
            .Setup(x => x.SearchUsersAsync(Realm, null, 0, 10, AccessToken, null, false))
            .ReturnsAsync([
                new KeycloakUserResponse { Id = "u1", Username = "alice" },
                new KeycloakUserResponse { Id = "u2", Username = "bob" }
            ]);
        _keycloakApiMock
            .Setup(x => x.GetUserGroupsAsync(Realm, It.IsAny<string>(), AccessToken))
            .ReturnsAsync([]);

        // Act
        var (users, _) = await _sut.GetUsersAsync(
            searchText: null,
            groupName: null,
            enabled: null,
            pageNumber: 1,
            pageSize: 10,
            excludeUserId: "u1",
            cancellationToken: CancellationToken.None);

        // Assert
        users.Should().ContainSingle();
        users[0].Id.Should().Be("u2");
    }

    [Fact]
    public async Task GetUsersAsync_ShouldExcludeAdminGroup_WhenExcludeAdminGroupNameProvided()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetUsersCountAsync(Realm, null, AccessToken, null))
            .ReturnsAsync(2);
        _keycloakApiMock
            .Setup(x => x.SearchUsersAsync(Realm, null, 0, 10, AccessToken, null, false))
            .ReturnsAsync([
                new KeycloakUserResponse { Id = "u1", Username = "admin-user" },
                new KeycloakUserResponse { Id = "u2", Username = "regular-user" }
            ]);
        _keycloakApiMock
            .Setup(x => x.GetUserGroupsAsync(Realm, "u1", AccessToken))
            .ReturnsAsync([new KeycloakGroupResponse { Id = "g1", Name = "system-admin", Path = "/system-admin" }]);
        _keycloakApiMock
            .Setup(x => x.GetUserGroupsAsync(Realm, "u2", AccessToken))
            .ReturnsAsync([new KeycloakGroupResponse { Id = "g2", Name = "users", Path = "/users" }]);

        // Act
        var (users, _) = await _sut.GetUsersAsync(
            searchText: null,
            groupName: null,
            enabled: null,
            pageNumber: 1,
            pageSize: 10,
            excludeAdminGroupName: "system-admin",
            cancellationToken: CancellationToken.None);

        // Assert
        users.Should().ContainSingle();
        users[0].Username.Should().Be("regular-user");
    }

    [Fact]
    public async Task GetUsersAsync_ShouldThrowInfrastructureException_WhenApiCallFails()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetUsersCountAsync(Realm, null, AccessToken, null))
            .ThrowsAsync(new HttpRequestException("server error"));

        // Act
        var act = () => _sut.GetUsersAsync(
            searchText: null,
            groupName: null,
            enabled: null,
            pageNumber: 1,
            pageSize: 10,
            cancellationToken: CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.FailedToGetUsers);
    }

    #endregion

    #region UpdateUserAsync

    [Fact]
    public async Task UpdateUserAsync_ShouldCallUpdateUser_WhenNoGroupsOrAvatar()
    {
        // Arrange
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.UpdateUserAsync(Realm, userId, It.IsAny<KeycloakUpdateUserRequest>(), AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateUserAsync(userId, "John", "Doe", null, true, null, cancellationToken: CancellationToken.None);

        // Assert
        _keycloakApiMock.Verify(
            x => x.UpdateUserAsync(Realm, userId, It.Is<KeycloakUpdateUserRequest>(r =>
                r.FirstName == "John" && r.LastName == "Doe" && r.Enabled == true && r.Attributes == null),
                AccessToken),
            Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldSetAvatarAttribute_WhenAvatarUrlProvided()
    {
        // Arrange
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.UpdateUserAsync(Realm, userId, It.IsAny<KeycloakUpdateUserRequest>(), AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateUserAsync(userId, "John", "Doe", null, true, null, avatarUrl: "http://img/avatar.png",
            cancellationToken: CancellationToken.None);

        // Assert
        _keycloakApiMock.Verify(
            x => x.UpdateUserAsync(Realm, userId, It.Is<KeycloakUpdateUserRequest>(r =>
                r.Attributes != null && r.Attributes.ContainsKey("avatarUrl")),
                AccessToken),
            Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldSyncGroups_WhenGroupNamesProvided()
    {
        // Arrange
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.UpdateUserAsync(Realm, userId, It.IsAny<KeycloakUpdateUserRequest>(), AccessToken))
            .Returns(Task.CompletedTask);
        _keycloakApiMock
            .Setup(x => x.GetUserGroupsAsync(Realm, userId, AccessToken))
            .ReturnsAsync([new KeycloakGroupResponse { Id = "g1", Name = "OldGroup", Path = "/OldGroup" }]);
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ReturnsAsync([
                new KeycloakGroupResponse { Id = "g1", Name = "OldGroup", Path = "/OldGroup" },
                new KeycloakGroupResponse { Id = "g2", Name = "NewGroup", Path = "/NewGroup" }
            ]);
        _keycloakApiMock
            .Setup(x => x.RemoveUserFromGroupAsync(Realm, userId, "g1", AccessToken))
            .Returns(Task.CompletedTask);
        _keycloakApiMock
            .Setup(x => x.AssignUserToGroupAsync(Realm, userId, "g2", AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateUserAsync(userId, "John", "Doe", null, true, ["NewGroup"],
            cancellationToken: CancellationToken.None);

        // Assert
        _keycloakApiMock.Verify(
            x => x.RemoveUserFromGroupAsync(Realm, userId, "g1", AccessToken), Times.Once);
        _keycloakApiMock.Verify(
            x => x.AssignUserToGroupAsync(Realm, userId, "g2", AccessToken), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldThrowInfrastructureException_WithUserNotFound_WhenApiReturns404()
    {
        // Arrange
        const string userId = "missing-user";
        SetupGetAccessToken();
        var apiEx = await CreateApiException(HttpStatusCode.NotFound);
        _keycloakApiMock
            .Setup(x => x.UpdateUserAsync(Realm, userId, It.IsAny<KeycloakUpdateUserRequest>(), AccessToken))
            .ThrowsAsync(apiEx);

        // Act
        var act = () => _sut.UpdateUserAsync(userId, "John", "Doe", null, true, null,
            cancellationToken: CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.UserNotFound);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldThrowInfrastructureException_WhenApiCallFails()
    {
        // Arrange
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.UpdateUserAsync(Realm, userId, It.IsAny<KeycloakUpdateUserRequest>(), AccessToken))
            .ThrowsAsync(new HttpRequestException("server error"));

        // Act
        var act = () => _sut.UpdateUserAsync(userId, "John", "Doe", null, true, null,
            cancellationToken: CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.FailedToUpdateUser);
    }

    #endregion

    #region UpdateUserGroupsAsync

    [Fact]
    public async Task UpdateUserGroupsAsync_ShouldAddAndRemoveGroups()
    {
        // Arrange
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetUserByIdAsync(Realm, userId, AccessToken))
            .ReturnsAsync(new KeycloakUserResponse { Id = userId, Username = "johndoe" });
        _keycloakApiMock
            .Setup(x => x.GetUserGroupsAsync(Realm, userId, AccessToken))
            .ReturnsAsync([new KeycloakGroupResponse { Id = "g1", Name = "OldGroup", Path = "/OldGroup" }]);
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ReturnsAsync([
                new KeycloakGroupResponse { Id = "g1", Name = "OldGroup", Path = "/OldGroup" },
                new KeycloakGroupResponse { Id = "g2", Name = "NewGroup", Path = "/NewGroup" }
            ]);
        _keycloakApiMock
            .Setup(x => x.RemoveUserFromGroupAsync(Realm, userId, "g1", AccessToken))
            .Returns(Task.CompletedTask);
        _keycloakApiMock
            .Setup(x => x.AssignUserToGroupAsync(Realm, userId, "g2", AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateUserGroupsAsync(userId, ["NewGroup"], CancellationToken.None);

        // Assert
        _keycloakApiMock.Verify(
            x => x.RemoveUserFromGroupAsync(Realm, userId, "g1", AccessToken), Times.Once);
        _keycloakApiMock.Verify(
            x => x.AssignUserToGroupAsync(Realm, userId, "g2", AccessToken), Times.Once);
    }

    [Fact]
    public async Task UpdateUserGroupsAsync_ShouldThrowInfrastructureException_WhenGroupNotFound()
    {
        // Arrange
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetUserByIdAsync(Realm, userId, AccessToken))
            .ReturnsAsync(new KeycloakUserResponse { Id = userId });
        _keycloakApiMock
            .Setup(x => x.GetUserGroupsAsync(Realm, userId, AccessToken))
            .ReturnsAsync([]);
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ReturnsAsync([]);

        // Act
        var act = () => _sut.UpdateUserGroupsAsync(userId, ["NonExistentGroup"], CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.GroupNotFound);
    }

    [Fact]
    public async Task UpdateUserGroupsAsync_ShouldThrowInfrastructureException_WithUserNotFound_WhenApiReturns404()
    {
        // Arrange
        const string userId = "missing-user";
        SetupGetAccessToken();
        var apiEx = await CreateApiException(HttpStatusCode.NotFound);
        _keycloakApiMock
            .Setup(x => x.GetUserByIdAsync(Realm, userId, AccessToken))
            .ThrowsAsync(apiEx);

        // Act
        var act = () => _sut.UpdateUserGroupsAsync(userId, ["Group1"], CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.UserNotFound);
    }

    [Fact]
    public async Task UpdateUserGroupsAsync_ShouldThrowInfrastructureException_WhenApiCallFails()
    {
        // Arrange
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetUserByIdAsync(Realm, userId, AccessToken))
            .ThrowsAsync(new HttpRequestException("server error"));

        // Act
        var act = () => _sut.UpdateUserGroupsAsync(userId, ["Group1"], CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.FailedToAssignGroup);
    }

    #endregion

    #region CreateUserAsync — additional paths

    [Fact]
    public async Task CreateUserAsync_ShouldSetAvatarAttribute_WhenAvatarUrlProvided()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.CreateUserAsync(Realm, It.Is<KeycloakCreateUserRequest>(r =>
                r.Attributes != null && r.Attributes.ContainsKey("avatarUrl")), AccessToken))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));
        _keycloakApiMock
            .Setup(x => x.GetUsersAsync(Realm, "johndoe", true, AccessToken))
            .ReturnsAsync([new KeycloakUserResponse { Id = "u1", Username = "johndoe" }]);

        // Act
        var result = await _sut.CreateUserAsync(
            "johndoe", "j@e.com", "John", "Doe", null, "pass",
            avatarUrl: "http://img/avatar.png",
            cancellationToken: CancellationToken.None);

        // Assert
        result.Should().Be("u1");
        _keycloakApiMock.Verify(
            x => x.CreateUserAsync(Realm, It.Is<KeycloakCreateUserRequest>(r =>
                r.Attributes != null && r.Attributes.ContainsKey("avatarUrl")), AccessToken),
            Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowInfrastructureException_WhenResponseIsNotSuccess()
    {
        // Arrange
        SetupGetAccessToken();
        var badResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Bad request")
        };
        _keycloakApiMock
            .Setup(x => x.CreateUserAsync(Realm, It.IsAny<KeycloakCreateUserRequest>(), AccessToken))
            .ReturnsAsync(badResponse);

        // Act
        var act = () => _sut.CreateUserAsync(
            "johndoe", "j@e.com", null, null, null, "pass",
            cancellationToken: CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.FailedToCreateUser);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldAssignGroups_WhenGroupNamesProvided()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.CreateUserAsync(Realm, It.IsAny<KeycloakCreateUserRequest>(), AccessToken))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));
        _keycloakApiMock
            .Setup(x => x.GetUsersAsync(Realm, "johndoe", true, AccessToken))
            .ReturnsAsync([new KeycloakUserResponse { Id = "u1", Username = "johndoe" }]);
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ReturnsAsync([new KeycloakGroupResponse { Id = "g1", Name = "Developers", Path = "/Developers" }]);
        _keycloakApiMock
            .Setup(x => x.AssignUserToGroupAsync(Realm, "u1", "g1", AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateUserAsync(
            "johndoe", "j@e.com", "John", "Doe", null, "pass",
            groupNames: ["Developers"],
            cancellationToken: CancellationToken.None);

        // Assert
        result.Should().Be("u1");
        _keycloakApiMock.Verify(
            x => x.AssignUserToGroupAsync(Realm, "u1", "g1", AccessToken), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldCompensateAndRethrow_WhenGroupAssignmentFailsWithInfrastructureException()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.CreateUserAsync(Realm, It.IsAny<KeycloakCreateUserRequest>(), AccessToken))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));
        _keycloakApiMock
            .Setup(x => x.GetUsersAsync(Realm, "johndoe", true, AccessToken))
            .ReturnsAsync([new KeycloakUserResponse { Id = "u1", Username = "johndoe" }]);
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ReturnsAsync([]); // group not found → InfrastructureException
        _keycloakApiMock
            .Setup(x => x.DeleteUserAsync(Realm, "u1", AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        var act = () => _sut.CreateUserAsync(
            "johndoe", "j@e.com", null, null, null, "pass",
            groupNames: ["MissingGroup"],
            cancellationToken: CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InfrastructureException>();
        _keycloakApiMock.Verify(
            x => x.DeleteUserAsync(Realm, "u1", AccessToken), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowUnknownError_WhenRawExceptionOccurs()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.CreateUserAsync(Realm, It.IsAny<KeycloakCreateUserRequest>(), AccessToken))
            .ThrowsAsync(new HttpRequestException("network error"));

        // Act
        var act = () => _sut.CreateUserAsync(
            "johndoe", "j@e.com", null, null, null, "pass",
            cancellationToken: CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.UnknownError);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowCompensationFailed_WhenCompensationDeleteFails()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.CreateUserAsync(Realm, It.IsAny<KeycloakCreateUserRequest>(), AccessToken))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));
        _keycloakApiMock
            .Setup(x => x.GetUsersAsync(Realm, "johndoe", true, AccessToken))
            .ReturnsAsync([new KeycloakUserResponse { Id = "u1", Username = "johndoe" }]);
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ReturnsAsync([]);
        _keycloakApiMock
            .Setup(x => x.DeleteUserAsync(Realm, "u1", AccessToken))
            .ThrowsAsync(new HttpRequestException("delete failed"));

        // Act
        var act = () => _sut.CreateUserAsync(
            "johndoe", "j@e.com", null, null, null, "pass",
            groupNames: ["MissingGroup"],
            cancellationToken: CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.UserCreationCompensationFailed);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldNotCompensate_WhenConflictOccursBeforeUserIdRetrieved()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.CreateUserAsync(Realm, It.IsAny<KeycloakCreateUserRequest>(), AccessToken))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Conflict));

        // Act
        var act = () => _sut.CreateUserAsync(
            "existing", "e@e.com", null, null, null, "pass",
            cancellationToken: CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InfrastructureException>();
        _keycloakApiMock.Verify(
            x => x.DeleteUserAsync(Realm, It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldFindGroupInSubGroups_WhenGroupIsNested()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.CreateUserAsync(Realm, It.IsAny<KeycloakCreateUserRequest>(), AccessToken))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));
        _keycloakApiMock
            .Setup(x => x.GetUsersAsync(Realm, "johndoe", true, AccessToken))
            .ReturnsAsync([new KeycloakUserResponse { Id = "u1", Username = "johndoe" }]);
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ReturnsAsync([
                new KeycloakGroupResponse
                {
                    Id = "parent", Name = "Parent", Path = "/Parent",
                    SubGroups = [new KeycloakGroupResponse { Id = "child", Name = "NestedGroup", Path = "/Parent/NestedGroup" }]
                }
            ]);
        _keycloakApiMock
            .Setup(x => x.AssignUserToGroupAsync(Realm, "u1", "child", AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateUserAsync(
            "johndoe", "j@e.com", null, null, null, "pass",
            groupNames: ["NestedGroup"],
            cancellationToken: CancellationToken.None);

        // Assert
        result.Should().Be("u1");
        _keycloakApiMock.Verify(
            x => x.AssignUserToGroupAsync(Realm, "u1", "child", AccessToken), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowInfrastructureException_WhenGroupNotFoundDuringAssignment()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.CreateUserAsync(Realm, It.IsAny<KeycloakCreateUserRequest>(), AccessToken))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));
        _keycloakApiMock
            .Setup(x => x.GetUsersAsync(Realm, "johndoe", true, AccessToken))
            .ReturnsAsync([new KeycloakUserResponse { Id = "u1", Username = "johndoe" }]);
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ReturnsAsync([new KeycloakGroupResponse { Id = "g1", Name = "ExistingGroup", Path = "/ExistingGroup" }]);
        _keycloakApiMock
            .Setup(x => x.DeleteUserAsync(Realm, "u1", AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        var act = () => _sut.CreateUserAsync(
            "johndoe", "j@e.com", null, null, null, "pass",
            groupNames: ["MissingGroup"],
            cancellationToken: CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.GroupNotFound);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowInfrastructureException_WhenAssignGroupFails()
    {
        // Arrange
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.CreateUserAsync(Realm, It.IsAny<KeycloakCreateUserRequest>(), AccessToken))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));
        _keycloakApiMock
            .Setup(x => x.GetUsersAsync(Realm, "johndoe", true, AccessToken))
            .ReturnsAsync([new KeycloakUserResponse { Id = "u1", Username = "johndoe" }]);
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ReturnsAsync([new KeycloakGroupResponse { Id = "g1", Name = "Developers", Path = "/Developers" }]);
        _keycloakApiMock
            .Setup(x => x.AssignUserToGroupAsync(Realm, "u1", "g1", AccessToken))
            .ThrowsAsync(new HttpRequestException("assign failed"));
        _keycloakApiMock
            .Setup(x => x.DeleteUserAsync(Realm, "u1", AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        var act = () => _sut.CreateUserAsync(
            "johndoe", "j@e.com", null, null, null, "pass",
            groupNames: ["Developers"],
            cancellationToken: CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InfrastructureException>();
    }

    #endregion

    #region AddRolesToGroupAsync — additional

    [Fact]
    public async Task AddRolesToGroupAsync_ShouldThrowInfrastructureException_WhenApiCallFails()
    {
        // Arrange
        const string groupId = "group-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetRealmRolesAsync(Realm, AccessToken))
            .ThrowsAsync(new HttpRequestException("server error"));

        // Act
        var act = () => _sut.AddRolesToGroupAsync(groupId, ["admin"], CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.FailedToAddRoleToGroup);
    }

    #endregion

    #region RemoveRolesFromGroupAsync — additional

    [Fact]
    public async Task RemoveRolesFromGroupAsync_ShouldThrowInfrastructureException_WhenRoleNameNotFound()
    {
        // Arrange
        const string groupId = "group-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetRealmRolesAsync(Realm, AccessToken))
            .ReturnsAsync([new KeycloakRoleResponse { Id = "r1", Name = "admin" }]);

        // Act
        var act = () => _sut.RemoveRolesFromGroupAsync(groupId, ["nonexistent-role"], CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.RoleNotFound);
    }

    [Fact]
    public async Task RemoveRolesFromGroupAsync_ShouldThrowInfrastructureException_WhenApiCallFails()
    {
        // Arrange
        const string groupId = "group-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetRealmRolesAsync(Realm, AccessToken))
            .ThrowsAsync(new HttpRequestException("server error"));

        // Act
        var act = () => _sut.RemoveRolesFromGroupAsync(groupId, ["admin"], CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.FailedToRemoveRoleFromGroup);
    }

    #endregion

    #region GetUserByIdAsync — avatarUrl mapping

    [Fact]
    public async Task GetUserByIdAsync_ShouldMapAvatarUrl_WhenAttributePresent()
    {
        // Arrange
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetUserByIdAsync(Realm, userId, AccessToken))
            .ReturnsAsync(new KeycloakUserResponse
            {
                Id = userId,
                Username = "johndoe",
                Attributes = new Dictionary<string, List<string>>
                {
                    { "avatarUrl", ["http://img/avatar.png"] }
                }
            });
        _keycloakApiMock
            .Setup(x => x.GetUserGroupsAsync(Realm, userId, AccessToken))
            .ReturnsAsync([]);
        _keycloakApiMock
            .Setup(x => x.GetUserRealmRoleMappingsAsync(Realm, userId, AccessToken))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.GetUserByIdAsync(userId, CancellationToken.None);

        // Assert
        result.AvatarUrl.Should().Be("http://img/avatar.png");
    }

    #endregion

    #region UpdateUserGroupsAsync — subgroup discovery

    [Fact]
    public async Task UpdateUserGroupsAsync_ShouldFindGroupInSubGroups()
    {
        // Arrange
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetUserByIdAsync(Realm, userId, AccessToken))
            .ReturnsAsync(new KeycloakUserResponse { Id = userId });
        _keycloakApiMock
            .Setup(x => x.GetUserGroupsAsync(Realm, userId, AccessToken))
            .ReturnsAsync([]);
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ReturnsAsync([
                new KeycloakGroupResponse
                {
                    Id = "parent", Name = "Parent", Path = "/Parent",
                    SubGroups = [new KeycloakGroupResponse { Id = "child", Name = "NestedGroup", Path = "/Parent/NestedGroup" }]
                }
            ]);
        _keycloakApiMock
            .Setup(x => x.AssignUserToGroupAsync(Realm, userId, "child", AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateUserGroupsAsync(userId, ["NestedGroup"], CancellationToken.None);

        // Assert
        _keycloakApiMock.Verify(
            x => x.AssignUserToGroupAsync(Realm, userId, "child", AccessToken), Times.Once);
    }

    #endregion

    #region GetUserIdByUsernameAsync — via CreateUserAsync

    [Fact]
    public async Task CreateUserAsync_ShouldThrow_WhenUserNotRetrievableAfterCreation()
    {
        // Arrange — user created but GetUsers returns empty
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.CreateUserAsync(Realm, It.IsAny<KeycloakCreateUserRequest>(), AccessToken))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));
        _keycloakApiMock
            .Setup(x => x.GetUsersAsync(Realm, "johndoe", true, AccessToken))
            .ReturnsAsync([]);

        // Act
        var act = () => _sut.CreateUserAsync(
            "johndoe", "j@e.com", null, null, null, "pass",
            cancellationToken: CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.FailedToRetrieveUser);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrow_WhenGetUsersAsyncThrowsRawException()
    {
        // Arrange — GetUsersAsync throws raw exception → caught by generic catch in GetUserIdByUsernameAsync
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.CreateUserAsync(Realm, It.IsAny<KeycloakCreateUserRequest>(), AccessToken))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));
        _keycloakApiMock
            .Setup(x => x.GetUsersAsync(Realm, "johndoe", true, AccessToken))
            .ThrowsAsync(new HttpRequestException("network error"));

        // Act
        var act = () => _sut.CreateUserAsync(
            "johndoe", "j@e.com", null, null, null, "pass",
            cancellationToken: CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.FailedToRetrieveUser);
    }

    #endregion

    #region AssignGroupsToUserAsync — outer catch via CreateUserAsync

    [Fact]
    public async Task CreateUserAsync_ShouldThrow_WhenGetGroupsThrowsRawExceptionDuringAssignment()
    {
        // Arrange — GetGroupsAsync throws raw exception in AssignGroupsToUserAsync outer catch
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.CreateUserAsync(Realm, It.IsAny<KeycloakCreateUserRequest>(), AccessToken))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));
        _keycloakApiMock
            .Setup(x => x.GetUsersAsync(Realm, "johndoe", true, AccessToken))
            .ReturnsAsync([new KeycloakUserResponse { Id = "u1", Username = "johndoe" }]);
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ThrowsAsync(new HttpRequestException("network error"));
        _keycloakApiMock
            .Setup(x => x.DeleteUserAsync(Realm, "u1", AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        var act = () => _sut.CreateUserAsync(
            "johndoe", "j@e.com", null, null, null, "pass",
            groupNames: ["SomeGroup"],
            cancellationToken: CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InfrastructureException>();
        _keycloakApiMock.Verify(
            x => x.DeleteUserAsync(Realm, "u1", AccessToken), Times.Once);
    }

    #endregion

    #region SyncUserGroupsAsync — null name + group not found via UpdateUserAsync

    [Fact]
    public async Task UpdateUserAsync_ShouldSkipNullNameGroup_WhenSyncingGroups()
    {
        // Arrange — current group with null Name should be skipped (not removed)
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.UpdateUserAsync(Realm, userId, It.IsAny<KeycloakUpdateUserRequest>(), AccessToken))
            .Returns(Task.CompletedTask);
        _keycloakApiMock
            .Setup(x => x.GetUserGroupsAsync(Realm, userId, AccessToken))
            .ReturnsAsync([
                new KeycloakGroupResponse { Id = "g1", Name = null!, Path = "/" },
                new KeycloakGroupResponse { Id = "g2", Name = "OldGroup", Path = "/OldGroup" }
            ]);
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ReturnsAsync([
                new KeycloakGroupResponse { Id = "g3", Name = "NewGroup", Path = "/NewGroup" }
            ]);
        _keycloakApiMock
            .Setup(x => x.RemoveUserFromGroupAsync(Realm, userId, It.IsAny<string>(), AccessToken))
            .Returns(Task.CompletedTask);
        _keycloakApiMock
            .Setup(x => x.AssignUserToGroupAsync(Realm, userId, "g3", AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateUserAsync(userId, "John", "Doe", null, true, ["NewGroup"],
            cancellationToken: CancellationToken.None);

        // Assert — null-name group not removed, OldGroup removed, NewGroup added
        _keycloakApiMock.Verify(
            x => x.RemoveUserFromGroupAsync(Realm, userId, "g1", AccessToken), Times.Never);
        _keycloakApiMock.Verify(
            x => x.RemoveUserFromGroupAsync(Realm, userId, "g2", AccessToken), Times.Once);
        _keycloakApiMock.Verify(
            x => x.AssignUserToGroupAsync(Realm, userId, "g3", AccessToken), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldThrow_WhenGroupNotFoundDuringSync()
    {
        // Arrange — SyncUserGroupsAsync group not found
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.UpdateUserAsync(Realm, userId, It.IsAny<KeycloakUpdateUserRequest>(), AccessToken))
            .Returns(Task.CompletedTask);
        _keycloakApiMock
            .Setup(x => x.GetUserGroupsAsync(Realm, userId, AccessToken))
            .ReturnsAsync([]);
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ReturnsAsync([]);

        // Act
        var act = () => _sut.UpdateUserAsync(userId, "John", "Doe", null, true, ["MissingGroup"],
            cancellationToken: CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<InfrastructureException>();
        ex.Which.Message.Should().Be(MessageCode.GroupNotFound);
    }

    #endregion

    #region UpdateUserGroupsAsync — null name in current groups

    [Fact]
    public async Task UpdateUserGroupsAsync_ShouldSkipNullNameGroup_WhenCurrentGroupHasNullName()
    {
        // Arrange
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetUserByIdAsync(Realm, userId, AccessToken))
            .ReturnsAsync(new KeycloakUserResponse { Id = userId });
        _keycloakApiMock
            .Setup(x => x.GetUserGroupsAsync(Realm, userId, AccessToken))
            .ReturnsAsync([
                new KeycloakGroupResponse { Id = "g1", Name = null!, Path = "/" },
                new KeycloakGroupResponse { Id = "g2", Name = "OldGroup", Path = "/OldGroup" }
            ]);
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ReturnsAsync([
                new KeycloakGroupResponse { Id = "g3", Name = "NewGroup", Path = "/NewGroup" }
            ]);
        _keycloakApiMock
            .Setup(x => x.RemoveUserFromGroupAsync(Realm, userId, It.IsAny<string>(), AccessToken))
            .Returns(Task.CompletedTask);
        _keycloakApiMock
            .Setup(x => x.AssignUserToGroupAsync(Realm, userId, "g3", AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateUserGroupsAsync(userId, ["NewGroup"], CancellationToken.None);

        // Assert
        _keycloakApiMock.Verify(
            x => x.RemoveUserFromGroupAsync(Realm, userId, "g1", AccessToken), Times.Never);
        _keycloakApiMock.Verify(
            x => x.RemoveUserFromGroupAsync(Realm, userId, "g2", AccessToken), Times.Once);
    }

    #endregion

    #region FindGroupByName — deeply nested subgroup via UpdateUserGroupsAsync

    [Fact]
    public async Task UpdateUserGroupsAsync_ShouldFindGroupInDeeplyNestedSubGroups()
    {
        // Arrange — group in subgroup of subgroup: Level1 > Level2 > Target
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetUserByIdAsync(Realm, userId, AccessToken))
            .ReturnsAsync(new KeycloakUserResponse { Id = userId });
        _keycloakApiMock
            .Setup(x => x.GetUserGroupsAsync(Realm, userId, AccessToken))
            .ReturnsAsync([]);
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ReturnsAsync([
                new KeycloakGroupResponse
                {
                    Id = "lvl1", Name = "Level1", Path = "/Level1",
                    SubGroups =
                    [
                        new KeycloakGroupResponse
                        {
                            Id = "lvl2", Name = "Level2", Path = "/Level1/Level2",
                            SubGroups =
                            [
                                new KeycloakGroupResponse
                                    { Id = "target", Name = "DeepGroup", Path = "/Level1/Level2/DeepGroup" }
                            ]
                        }
                    ]
                }
            ]);
        _keycloakApiMock
            .Setup(x => x.AssignUserToGroupAsync(Realm, userId, "target", AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateUserGroupsAsync(userId, ["DeepGroup"], CancellationToken.None);

        // Assert
        _keycloakApiMock.Verify(
            x => x.AssignUserToGroupAsync(Realm, userId, "target", AccessToken), Times.Once);
    }

    #endregion

    #region MapToUserDto — groups null vs populated via GetUsersAsync

    [Fact]
    public async Task GetUsersAsync_ShouldMapUserWithoutGroups_WhenNoGroupFilter()
    {
        // Arrange — exercises MapToUserDto with groups=null → branch coverage for groups?.Select(...)
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.SearchUsersAsync(Realm, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                AccessToken, It.IsAny<bool?>(), It.IsAny<bool>()))
            .ReturnsAsync([new KeycloakUserResponse
            {
                Id = "u1", Username = "alice",
                Attributes = new Dictionary<string, List<string>>
                {
                    { "avatarUrl", ["http://cdn.test/a.png"] }
                }
            }]);
        _keycloakApiMock
            .Setup(x => x.GetUsersCountAsync(Realm, It.IsAny<string>(), AccessToken, It.IsAny<bool?>()))
            .ReturnsAsync(1);

        // Act
        var (users, totalCount) = await _sut.GetUsersAsync("alice", null, null, 1, 10);

        // Assert
        totalCount.Should().Be(1);
        users.Should().ContainSingle();
        users[0].AvatarUrl.Should().Be("http://cdn.test/a.png");
        users[0].Groups.Should().BeEmpty();
    }

    #endregion

    #region Constructor — null scopes

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenScopesIsNull()
    {
        // Arrange — section key exists with a value but no indexed children → Get<string[]>() returns null
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{ApiClientCfg.Keycloak.Section}:{ApiClientCfg.Keycloak.Realm}"] = Realm,
                [$"{ApiClientCfg.Keycloak.Section}:{ApiClientCfg.Keycloak.ClientId}"] = "svc-client",
                [$"{ApiClientCfg.Keycloak.Section}:{ApiClientCfg.Keycloak.ClientSecret}"] = "secret",
                [$"{ApiClientCfg.Keycloak.Section}:{ApiClientCfg.Keycloak.GrantType}"] = "client_credentials",
                [$"{ApiClientCfg.Keycloak.Section}:{ApiClientCfg.Keycloak.Scopes}"] = "",
            })
            .Build();

        // Act
        var act = () => new KeycloakService(
            _keycloakApiMock.Object,
            config,
            NullLogger<KeycloakService>.Instance);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region FindGroupByName — subgroups miss then find at same level

    [Fact]
    public async Task UpdateUserGroupsAsync_ShouldFindGroup_WhenFirstGroupHasSubGroupsThatDontMatch()
    {
        // Arrange — Parent has SubGroups (Unrelated), Target is a sibling at same level.
        // Exercises the FindGroupByName false branch: found is null after recursion, continue loop.
        const string userId = "user-123";
        SetupGetAccessToken();
        _keycloakApiMock
            .Setup(x => x.GetUserByIdAsync(Realm, userId, AccessToken))
            .ReturnsAsync(new KeycloakUserResponse { Id = userId });
        _keycloakApiMock
            .Setup(x => x.GetUserGroupsAsync(Realm, userId, AccessToken))
            .ReturnsAsync([]);
        _keycloakApiMock
            .Setup(x => x.GetGroupsAsync(Realm, null, AccessToken))
            .ReturnsAsync([
                new KeycloakGroupResponse
                {
                    Id = "parent", Name = "Parent", Path = "/Parent",
                    SubGroups = [new KeycloakGroupResponse { Id = "unrelated", Name = "Unrelated", Path = "/Parent/Unrelated" }]
                },
                new KeycloakGroupResponse { Id = "target", Name = "TargetGroup", Path = "/TargetGroup" }
            ]);
        _keycloakApiMock
            .Setup(x => x.AssignUserToGroupAsync(Realm, userId, "target", AccessToken))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateUserGroupsAsync(userId, ["TargetGroup"], CancellationToken.None);

        // Assert
        _keycloakApiMock.Verify(
            x => x.AssignUserToGroupAsync(Realm, userId, "target", AccessToken), Times.Once);
    }

    #endregion
}
