using Lab.Application.Dtos.AuthorRoles;
using Lab.Application.Features.AuthorRole.Commands.CreateAuthorRole;
using Lab.Application.Features.AuthorRole.Commands.DeleteAuthorRole;
using Lab.Application.Features.AuthorRole.Commands.UpdateAuthorRole;
using Lab.Application.Features.AuthorRole.Queries.GetAuthorRoleById;
using Lab.Application.Features.AuthorRole.Queries.GetAuthorRoles;
using Lab.Application.Models.Results;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;

namespace Lab.Application.Tests.Features.AuthorRole;

public class AuthorRoleIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "author_role_tests";

    #region CreateAuthorRole Tests

    [Fact]
    public async Task CreateAuthorRole_WithValidData_ShouldStoreAndReturnId()
    {
        // Arrange
        var handler = new CreateAuthorRoleCommandHandler(Session);
        var dto = new CreateAuthorRoleDto { Name = "Lead Author", Description = "Primary author" };
        var command = new CreateAuthorRoleCommand(dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();

        var stored = await Session.LoadAsync<AuthorRoleEntity>(result);
        stored.Should().NotBeNull();
        stored!.Name.Should().Be("Lead Author");
        stored.Description.Should().Be("Primary author");
    }

    [Fact]
    public async Task CreateAuthorRole_ShouldTrimName()
    {
        // Arrange
        var handler = new CreateAuthorRoleCommandHandler(Session);
        var dto = new CreateAuthorRoleDto { Name = "  Co Author  " };
        var command = new CreateAuthorRoleCommand(dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var stored = await Session.LoadAsync<AuthorRoleEntity>(result);
        stored.Should().NotBeNull();
        stored!.Name.Should().Be("Co Author");
    }

    [Fact]
    public async Task CreateAuthorRole_WithDuplicateName_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new CreateAuthorRoleCommandHandler(Session);
        var dto = new CreateAuthorRoleDto { Name = "Reviewer" };
        await handler.Handle(new CreateAuthorRoleCommand(dto), CancellationToken.None);

        // Act
        var act = () => handler.Handle(new CreateAuthorRoleCommand(dto), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task CreateAuthorRole_WithDuplicateNameDifferentCase_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new CreateAuthorRoleCommandHandler(Session);
        await handler.Handle(new CreateAuthorRoleCommand(new CreateAuthorRoleDto { Name = "Editor" }), CancellationToken.None);

        // Act
        var act = () => handler.Handle(new CreateAuthorRoleCommand(new CreateAuthorRoleDto { Name = "  editor  " }), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region UpdateAuthorRole Tests

    [Fact]
    public async Task UpdateAuthorRole_WithValidData_ShouldUpdateNameAndDescription()
    {
        // Arrange
        var createHandler = new CreateAuthorRoleCommandHandler(Session);
        var id = await createHandler.Handle(
            new CreateAuthorRoleCommand(new CreateAuthorRoleDto { Name = "Old Role", Description = "Old desc" }),
            CancellationToken.None);

        var updateHandler = new UpdateAuthorRoleCommandHandler(Session);
        var command = new UpdateAuthorRoleCommand(id, new UpdateAuthorRoleDto { Name = "New Role", Description = "New desc" });

        // Act
        var result = await updateHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(id);

        var updated = await Session.LoadAsync<AuthorRoleEntity>(id);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("New Role");
        updated.Description.Should().Be("New desc");
    }

    [Fact]
    public async Task UpdateAuthorRole_WithNullName_ShouldKeepExistingName()
    {
        // Arrange
        var createHandler = new CreateAuthorRoleCommandHandler(Session);
        var id = await createHandler.Handle(
            new CreateAuthorRoleCommand(new CreateAuthorRoleDto { Name = "Keep Me", Description = "Original" }),
            CancellationToken.None);

        var updateHandler = new UpdateAuthorRoleCommandHandler(Session);
        var command = new UpdateAuthorRoleCommand(id, new UpdateAuthorRoleDto { Name = null, Description = "Updated" });

        // Act
        await updateHandler.Handle(command, CancellationToken.None);

        // Assert
        var updated = await Session.LoadAsync<AuthorRoleEntity>(id);
        updated!.Name.Should().Be("Keep Me");
        updated.Description.Should().Be("Updated");
    }

    [Fact]
    public async Task UpdateAuthorRole_WithNonExistentId_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new UpdateAuthorRoleCommandHandler(Session);
        var command = new UpdateAuthorRoleCommand(Guid.NewGuid(), new UpdateAuthorRoleDto { Name = "any" });

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region DeleteAuthorRole Tests

    [Fact]
    public async Task DeleteAuthorRole_WithExistingRole_ShouldRemoveFromStore()
    {
        // Arrange
        var createHandler = new CreateAuthorRoleCommandHandler(Session);
        var id = await createHandler.Handle(
            new CreateAuthorRoleCommand(new CreateAuthorRoleDto { Name = "To Delete" }),
            CancellationToken.None);

        var deleteHandler = new DeleteAuthorRoleCommandHandler(Session);

        // Act
        await deleteHandler.Handle(new DeleteAuthorRoleCommand(id), CancellationToken.None);

        // Assert
        var deleted = await Session.LoadAsync<AuthorRoleEntity>(id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAuthorRole_WithNonExistentId_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new DeleteAuthorRoleCommandHandler(Session);

        // Act
        var act = () => handler.Handle(new DeleteAuthorRoleCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region GetAuthorRoleById Tests

    [Fact]
    public async Task GetAuthorRoleById_WithExistingRole_ShouldReturnMappedResult()
    {
        // Arrange
        var createHandler = new CreateAuthorRoleCommandHandler(Session);
        var id = await createHandler.Handle(
            new CreateAuthorRoleCommand(new CreateAuthorRoleDto { Name = "Query Role", Description = "Desc" }),
            CancellationToken.None);

        var queryHandler = new GetAuthorRoleByIdQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(new GetAuthorRoleByIdQuery(id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AuthorRole.Should().NotBeNull();
        result.AuthorRole.Id.Should().Be(id);
        result.AuthorRole.Name.Should().Be("Query Role");
    }

    [Fact]
    public async Task GetAuthorRoleById_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        var handler = new GetAuthorRoleByIdQueryHandler(Session, Mapper);

        // Act
        var act = () => handler.Handle(new GetAuthorRoleByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetAuthorRoles Tests

    [Fact]
    public async Task GetAuthorRoles_WithNoFilter_ShouldReturnAllRoles()
    {
        // Arrange
        var createHandler = new CreateAuthorRoleCommandHandler(Session);
        await createHandler.Handle(new CreateAuthorRoleCommand(new CreateAuthorRoleDto { Name = "Role A" }), CancellationToken.None);
        await createHandler.Handle(new CreateAuthorRoleCommand(new CreateAuthorRoleDto { Name = "Role B" }), CancellationToken.None);
        await createHandler.Handle(new CreateAuthorRoleCommand(new CreateAuthorRoleDto { Name = "Role C" }), CancellationToken.None);

        var queryHandler = new GetAuthorRolesQueryHandler(Session, Mapper);
        var query = new GetAuthorRolesQuery(null, new PaginationRequest(1, 10));

        // Act
        var result = await queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAuthorRoles_WithNameFilter_ShouldReturnMatchingRoles()
    {
        // Arrange
        var createHandler = new CreateAuthorRoleCommandHandler(Session);
        await createHandler.Handle(new CreateAuthorRoleCommand(new CreateAuthorRoleDto { Name = "Lead Author" }), CancellationToken.None);
        await createHandler.Handle(new CreateAuthorRoleCommand(new CreateAuthorRoleDto { Name = "Lead Editor" }), CancellationToken.None);
        await createHandler.Handle(new CreateAuthorRoleCommand(new CreateAuthorRoleDto { Name = "Reviewer" }), CancellationToken.None);

        var queryHandler = new GetAuthorRolesQueryHandler(Session, Mapper);
        var query = new GetAuthorRolesQuery("lead", new PaginationRequest(1, 10));

        // Act
        var result = await queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAuthorRoles_WithPagination_ShouldReturnPagedResults()
    {
        // Arrange
        var createHandler = new CreateAuthorRoleCommandHandler(Session);
        for (var i = 1; i <= 5; i++)
            await createHandler.Handle(new CreateAuthorRoleCommand(new CreateAuthorRoleDto { Name = $"Paged Role {i}" }), CancellationToken.None);

        var queryHandler = new GetAuthorRolesQueryHandler(Session, Mapper);
        var query = new GetAuthorRolesQuery(null, new PaginationRequest(1, 2));

        // Act
        var result = await queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Paging.Should().NotBeNull();
    }

    #endregion
}
