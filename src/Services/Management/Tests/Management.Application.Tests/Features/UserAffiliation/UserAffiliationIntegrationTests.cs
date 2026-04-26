using Management.Application.Dtos.Affiliations;
using Management.Application.Dtos.UserAffiliations;
using Management.Application.Features.Affiliation.Commands;
using Management.Application.Features.UserAffiliation.Commands;
using Management.Application.Features.UserAffiliation.Queries;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.UserAffiliation;

public class UserAffiliationIntegrationTests : ManagementMartenTestBase
{
    protected override string SchemaName => "user_affiliation_tests";

    private async Task<Guid> SeedAffiliation(string name = "Test University", string rorId = "ror-test")
    {
        var handler = new CreateAffiliationCommandHandler(Session);
        return await handler.Handle(
            new CreateAffiliationCommand(new CreateAffiliationDto { Name = name, RorId = rorId }),
            CancellationToken.None);
    }

    private async Task<Guid> SeedUserAffiliation(Guid userId, Guid affiliationId,
        string? department = null, string? position = null,
        int? startYear = null, int? endYear = null)
    {
        var handler = new CreateUserAffiliationCommandHandler(Session);
        return await handler.Handle(
            new CreateUserAffiliationCommand(new CreateUserAffiliationDto
            {
                UserId = userId,
                AffiliationId = affiliationId,
                Department = department,
                Position = position,
                AffiliationStartYear = startYear,
                AffiliationEndYear = endYear
            }),
            CancellationToken.None);
    }

    private async Task<(Guid MemberId, Guid UserId)> SeedMember()
    {
        var userId = Guid.NewGuid();
        var member = MemberEntity.Create(Guid.NewGuid(), userId, Guid.NewGuid(), "member", DateTimeOffset.UtcNow);
        Session.Store(member);
        await Session.SaveChangesAsync(CancellationToken.None);
        return (member.Id, userId);
    }

    #region CreateUserAffiliation Tests

    [Fact]
    public async Task CreateUserAffiliation_WithValidData_ShouldStoreAndReturnId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var affiliationId = Guid.NewGuid();
        var handler = new CreateUserAffiliationCommandHandler(Session);
        var dto = new CreateUserAffiliationDto
        {
            UserId = userId,
            AffiliationId = affiliationId,
            Department = "CS",
            Position = "Professor",
            AffiliationStartYear = 2020,
            AffiliationEndYear = 2024
        };

        // Act
        var result = await handler.Handle(new CreateUserAffiliationCommand(dto), CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        var stored = await Session.LoadAsync<UserAffiliationEntity>(result);
        stored.Should().NotBeNull();
        stored!.UserId.Should().Be(userId);
        stored.AffiliationId.Should().Be(affiliationId);
        stored.Department.Should().Be("CS");
        stored.Position.Should().Be("Professor");
        stored.AffiliationStartYear.Should().Be(2020);
        stored.AffiliationEndYear.Should().Be(2024);
    }

    #endregion

    #region UpdateUserAffiliation Tests

    [Fact]
    public async Task UpdateUserAffiliation_WithValidData_ShouldUpdateFields()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var affiliationId = await SeedAffiliation();
        var id = await SeedUserAffiliation(userId, affiliationId, "Old Dept", "Old Position");

        var newUserId = Guid.NewGuid();
        var newAffiliationId = Guid.NewGuid();
        var updateHandler = new UpdateUserAffiliationCommandHandler(Session);
        var dto = new UpdateUserAffiliationDto
        {
            UserId = newUserId,
            AffiliationId = newAffiliationId,
            Department = "New Dept",
            Position = "New Position",
            AffiliationStartYear = 2021,
            AffiliationEndYear = 2025
        };

        // Act
        var result = await updateHandler.Handle(new UpdateUserAffiliationCommand(id, dto), CancellationToken.None);

        // Assert
        result.Should().Be(id);
        var updated = await Session.LoadAsync<UserAffiliationEntity>(id);
        updated.Should().NotBeNull();
        updated!.UserId.Should().Be(newUserId);
        updated.AffiliationId.Should().Be(newAffiliationId);
        updated.Department.Should().Be("New Dept");
        updated.Position.Should().Be("New Position");
        updated.AffiliationStartYear.Should().Be(2021);
        updated.AffiliationEndYear.Should().Be(2025);
    }

    [Fact]
    public async Task UpdateUserAffiliation_WithNonExistentId_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new UpdateUserAffiliationCommandHandler(Session);
        var dto = new UpdateUserAffiliationDto { Department = "Any" };

        // Act
        var act = () => handler.Handle(new UpdateUserAffiliationCommand(Guid.NewGuid(), dto), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region DeleteUserAffiliation Tests

    [Fact]
    public async Task DeleteUserAffiliation_WithExistingEntity_ShouldRemoveFromStore()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var affiliationId = Guid.NewGuid();
        var id = await SeedUserAffiliation(userId, affiliationId);

        var deleteHandler = new DeleteUserAffiliationCommandHandler(Session);

        // Act
        await deleteHandler.Handle(new DeleteUserAffiliationCommand(id), CancellationToken.None);

        // Assert
        var deleted = await Session.LoadAsync<UserAffiliationEntity>(id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteUserAffiliation_WithNonExistentId_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new DeleteUserAffiliationCommandHandler(Session);

        // Act
        var act = () => handler.Handle(new DeleteUserAffiliationCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region GetUserAffiliationById Tests

    [Fact]
    public async Task GetUserAffiliationById_WhenAffiliationEntityExists_ShouldReturnMappedResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var affiliationId = await SeedAffiliation("Harvard University", "ror-harvard");
        var id = await SeedUserAffiliation(userId, affiliationId, "Law", "Dean");

        var queryHandler = new GetUserAffiliationByIdQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(new GetUserAffiliationByIdQuery(id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.UserId.Should().Be(userId);
        result.Department.Should().Be("Law");
        result.Affiliation.Should().NotBeNull();
        result.Affiliation!.Name.Should().Be("Harvard University");
    }

    [Fact]
    public async Task GetUserAffiliationById_WhenAffiliationEntityNotFound_ShouldReturnNullAffiliation()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var nonExistentAffiliationId = Guid.NewGuid();
        var id = await SeedUserAffiliation(userId, nonExistentAffiliationId);

        var queryHandler = new GetUserAffiliationByIdQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(new GetUserAffiliationByIdQuery(id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Affiliation.Should().BeNull();
    }

    [Fact]
    public async Task GetUserAffiliationById_WithNonExistentId_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new GetUserAffiliationByIdQueryHandler(Session, Mapper);

        // Act
        var act = () => handler.Handle(new GetUserAffiliationByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region GetUserAffiliations Tests

    [Fact]
    public async Task GetUserAffiliations_WithNoAffiliations_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var queryHandler = new GetUserAffiliationsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(new GetUserAffiliationsQuery(userId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserAffiliations_WhenAffiliationEntityExists_ShouldReturnMappedAffiliation()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var affiliationId = await SeedAffiliation("Cambridge University", "ror-cambridge");
        await SeedUserAffiliation(userId, affiliationId, "Physics");

        var queryHandler = new GetUserAffiliationsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(new GetUserAffiliationsQuery(userId), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Affiliation.Should().NotBeNull();
        result[0].Affiliation!.Name.Should().Be("Cambridge University");
    }

    [Fact]
    public async Task GetUserAffiliations_WhenAffiliationEntityNotFound_ShouldReturnNullAffiliation()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var nonExistentAffiliationId = Guid.NewGuid();
        await SeedUserAffiliation(userId, nonExistentAffiliationId);

        var queryHandler = new GetUserAffiliationsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(new GetUserAffiliationsQuery(userId), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Affiliation.Should().BeNull();
    }

    [Fact]
    public async Task GetUserAffiliations_ShouldReturnOnlyForRequestedUser()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var affiliationId = Guid.NewGuid();
        await SeedUserAffiliation(userId1, affiliationId);
        await SeedUserAffiliation(userId2, affiliationId);

        var queryHandler = new GetUserAffiliationsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(new GetUserAffiliationsQuery(userId1), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].UserId.Should().Be(userId1);
    }

    #endregion

    #region GetMemberAffiliations Tests

    [Fact]
    public async Task GetMemberAffiliations_MemberNotFound_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new GetMemberAffiliationsQueryHandler(Session, Mapper);

        // Act
        var act = () => handler.Handle(
            new GetMemberAffiliationsQuery(Guid.NewGuid(), new PaginationRequest(1, 10), null),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task GetMemberAffiliations_WithNoUserAffiliations_ShouldReturnEmptyResult()
    {
        // Arrange
        var (memberId, _) = await SeedMember();
        var queryHandler = new GetMemberAffiliationsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(
            new GetMemberAffiliationsQuery(memberId, new PaginationRequest(1, 10), null),
            CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMemberAffiliations_WithNoNameFilter_ShouldReturnAllAffiliations()
    {
        // Arrange
        var (memberId, userId) = await SeedMember();
        var affId1 = await SeedAffiliation("Oxford", "ror-oxford");
        var affId2 = await SeedAffiliation("Yale", "ror-yale");
        await SeedUserAffiliation(userId, affId1);
        await SeedUserAffiliation(userId, affId2);

        var queryHandler = new GetMemberAffiliationsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(
            new GetMemberAffiliationsQuery(memberId, new PaginationRequest(1, 10), null),
            CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetMemberAffiliations_WithNoNameFilter_WhenAffiliationEntityMissing_ShouldIncludeWithNullAffiliation()
    {
        // Arrange
        var (memberId, userId) = await SeedMember();
        var nonExistentAffiliationId = Guid.NewGuid();
        await SeedUserAffiliation(userId, nonExistentAffiliationId);

        var queryHandler = new GetMemberAffiliationsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(
            new GetMemberAffiliationsQuery(memberId, new PaginationRequest(1, 10), null),
            CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items[0].Affiliation.Should().BeNull();
    }

    [Fact]
    public async Task GetMemberAffiliations_WithNameFilterMatching_ShouldReturnFilteredAffiliations()
    {
        // Arrange
        var (memberId, userId) = await SeedMember();
        var affId1 = await SeedAffiliation("Oxford University", "ror-oxford");
        var affId2 = await SeedAffiliation("Cambridge University", "ror-cambridge");
        await SeedUserAffiliation(userId, affId1);
        await SeedUserAffiliation(userId, affId2);

        var queryHandler = new GetMemberAffiliationsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(
            new GetMemberAffiliationsQuery(memberId, new PaginationRequest(1, 10), "oxford"),
            CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items[0].Affiliation!.Name.Should().Be("Oxford University");
    }

    [Fact]
    public async Task GetMemberAffiliations_WithNameFilterNotMatching_ShouldExcludeEntry()
    {
        // Arrange
        var (memberId, userId) = await SeedMember();
        var affId = await SeedAffiliation("Stanford", "ror-stanford");
        await SeedUserAffiliation(userId, affId);

        var queryHandler = new GetMemberAffiliationsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(
            new GetMemberAffiliationsQuery(memberId, new PaginationRequest(1, 10), "oxford"),
            CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMemberAffiliations_WithNameFilter_WhenAffiliationEntityMissing_ShouldSkipEntry()
    {
        // Arrange
        var (memberId, userId) = await SeedMember();
        var nonExistentAffiliationId = Guid.NewGuid();
        await SeedUserAffiliation(userId, nonExistentAffiliationId);

        var queryHandler = new GetMemberAffiliationsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(
            new GetMemberAffiliationsQuery(memberId, new PaginationRequest(1, 10), "any"),
            CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMemberAffiliations_WithNameFilter_WhenAffiliationNameIsNull_ShouldSkipEntry()
    {
        // Arrange
        var (memberId, userId) = await SeedMember();
        var affiliationId = Guid.NewGuid();

        // Store an affiliation entity with a null Name to exercise the affiliation.Name is null branch
        var affiliationWithNullName = new AffiliationEntity
        {
            Id = affiliationId,
            RorId = "ror-null-name",
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(affiliationWithNullName);
        await Session.SaveChangesAsync(CancellationToken.None);

        await SeedUserAffiliation(userId, affiliationId);

        var queryHandler = new GetMemberAffiliationsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(
            new GetMemberAffiliationsQuery(memberId, new PaginationRequest(1, 10), "any"),
            CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMemberAffiliations_WithPagination_ShouldReturnPagedResults()
    {
        // Arrange
        var (memberId, userId) = await SeedMember();
        for (var i = 1; i <= 5; i++)
        {
            var affId = await SeedAffiliation($"University {i}", $"ror-{i}");
            await SeedUserAffiliation(userId, affId);
        }

        var queryHandler = new GetMemberAffiliationsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(
            new GetMemberAffiliationsQuery(memberId, new PaginationRequest(1, 2), null),
            CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Paging.Should().NotBeNull();
    }

    #endregion
}
