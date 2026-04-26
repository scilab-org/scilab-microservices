using Management.Application.Dtos.Domains;
using Management.Application.Features.Domain.Commands;
using Management.Application.Features.Domain.Queries;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Domain;

public class DomainIntegrationTests : ManagementMartenTestBase
{
    protected override string SchemaName => "domain_tests";

    #region CreateDomain Tests

    [Fact]
    public async Task CreateDomain_WithValidName_ShouldStoreAndReturnId()
    {
        // Arrange
        var handler = new CreateDomainCommandHandler(Session);
        var dto = new CreateDomainDto { Name = "Computer Science" };

        // Act
        var result = await handler.Handle(new CreateDomainCommand(dto), CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        var stored = await Session.LoadAsync<DomainEntity>(result);
        stored.Should().NotBeNull();
        stored!.Name.Should().Be("Computer Science");
    }

    #endregion

    #region UpdateDomain Tests

    [Fact]
    public async Task UpdateDomain_WithValidData_ShouldUpdateName()
    {
        // Arrange
        var createHandler = new CreateDomainCommandHandler(Session);
        var id = await createHandler.Handle(
            new CreateDomainCommand(new CreateDomainDto { Name = "Old Domain" }),
            CancellationToken.None);

        var updateHandler = new UpdateDomainCommandHandler(Session);

        // Act
        var result = await updateHandler.Handle(
            new UpdateDomainCommand(id, new UpdateDomainDto { Name = "New Domain" }),
            CancellationToken.None);

        // Assert
        result.Should().Be(id);
        var updated = await Session.LoadAsync<DomainEntity>(id);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("New Domain");
    }

    [Fact]
    public async Task UpdateDomain_WithNonExistentId_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new UpdateDomainCommandHandler(Session);

        // Act
        var act = () => handler.Handle(
            new UpdateDomainCommand(Guid.NewGuid(), new UpdateDomainDto { Name = "Any" }),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region DeleteDomain Tests

    [Fact]
    public async Task DeleteDomain_WithExistingDomain_ShouldRemoveFromStore()
    {
        // Arrange
        var createHandler = new CreateDomainCommandHandler(Session);
        var id = await createHandler.Handle(
            new CreateDomainCommand(new CreateDomainDto { Name = "To Delete" }),
            CancellationToken.None);

        var deleteHandler = new DeleteDomainCommandHandler(Session);

        // Act
        await deleteHandler.Handle(new DeleteDomainCommand(id), CancellationToken.None);

        // Assert
        var deleted = await Session.LoadAsync<DomainEntity>(id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteDomain_WithNonExistentId_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new DeleteDomainCommandHandler(Session);

        // Act
        var act = () => handler.Handle(new DeleteDomainCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region GetDomainById Tests

    [Fact]
    public async Task GetDomainById_WithExistingDomain_ShouldReturnMappedResult()
    {
        // Arrange
        var createHandler = new CreateDomainCommandHandler(Session);
        var id = await createHandler.Handle(
            new CreateDomainCommand(new CreateDomainDto { Name = "Query Domain" }),
            CancellationToken.None);

        var queryHandler = new GetDomainByIdQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(new GetDomainByIdQuery(id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Name.Should().Be("Query Domain");
    }

    [Fact]
    public async Task GetDomainById_WithNonExistentId_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new GetDomainByIdQueryHandler(Session, Mapper);

        // Act
        var act = () => handler.Handle(new GetDomainByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region GetDomains Tests

    [Fact]
    public async Task GetDomains_WithNoFilter_ShouldReturnAllDomains()
    {
        // Arrange
        var createHandler = new CreateDomainCommandHandler(Session);
        await createHandler.Handle(new CreateDomainCommand(new CreateDomainDto { Name = "Domain A" }), CancellationToken.None);
        await createHandler.Handle(new CreateDomainCommand(new CreateDomainDto { Name = "Domain B" }), CancellationToken.None);
        await createHandler.Handle(new CreateDomainCommand(new CreateDomainDto { Name = "Domain C" }), CancellationToken.None);

        var queryHandler = new GetDomainsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(new GetDomainsQuery(new PaginationRequest(1, 10), null), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetDomains_WithNameFilter_ShouldReturnMatchingDomains()
    {
        // Arrange
        var createHandler = new CreateDomainCommandHandler(Session);
        await createHandler.Handle(new CreateDomainCommand(new CreateDomainDto { Name = "Computer Science" }), CancellationToken.None);
        await createHandler.Handle(new CreateDomainCommand(new CreateDomainDto { Name = "Computer Engineering" }), CancellationToken.None);
        await createHandler.Handle(new CreateDomainCommand(new CreateDomainDto { Name = "Mathematics" }), CancellationToken.None);

        var queryHandler = new GetDomainsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(new GetDomainsQuery(new PaginationRequest(1, 10), "computer"), CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(d => d.Name!.ToLower().Should().Contain("computer"));
    }

    [Fact]
    public async Task GetDomains_WithWhitespaceNameFilter_ShouldReturnAllDomains()
    {
        // Arrange
        var createHandler = new CreateDomainCommandHandler(Session);
        await createHandler.Handle(new CreateDomainCommand(new CreateDomainDto { Name = "Physics" }), CancellationToken.None);

        var queryHandler = new GetDomainsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(new GetDomainsQuery(new PaginationRequest(1, 10), "   "), CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetDomains_WithPagination_ShouldReturnPagedResults()
    {
        // Arrange
        var createHandler = new CreateDomainCommandHandler(Session);
        for (var i = 1; i <= 5; i++)
            await createHandler.Handle(new CreateDomainCommand(new CreateDomainDto { Name = $"Domain {i}" }), CancellationToken.None);

        var queryHandler = new GetDomainsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(new GetDomainsQuery(new PaginationRequest(1, 2), null), CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Paging.Should().NotBeNull();
    }

    #endregion
}
