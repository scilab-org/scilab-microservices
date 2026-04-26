using Management.Application.Dtos.Affiliations;
using Management.Application.Features.Affiliation.Commands;
using Management.Application.Features.Affiliation.Queries;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Affiliation;

public class AffiliationIntegrationTests : ManagementMartenTestBase
{
    protected override string SchemaName => "affiliation_tests";

    #region CreateAffiliation Tests

    [Fact]
    public async Task CreateAffiliation_WithValidData_ShouldStoreAndReturnId()
    {
        // Arrange
        var handler = new CreateAffiliationCommandHandler(Session);
        var dto = new CreateAffiliationDto { Name = "MIT", RorId = "https://ror.org/042nb2s44" };

        // Act
        var result = await handler.Handle(new CreateAffiliationCommand(dto), CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        var stored = await Session.LoadAsync<AffiliationEntity>(result);
        stored.Should().NotBeNull();
        stored!.Name.Should().Be("MIT");
        stored.RorId.Should().Be("https://ror.org/042nb2s44");
    }

    [Fact]
    public async Task CreateAffiliation_WithOptionalFields_ShouldStoreAllFields()
    {
        // Arrange
        var handler = new CreateAffiliationCommandHandler(Session);
        var dto = new CreateAffiliationDto
        {
            Name = "Massachusetts Institute of Technology",
            ShortName = "MIT",
            RorId = "https://ror.org/042nb2s44",
            RorUrl = "https://ror.org/042nb2s44"
        };

        // Act
        var result = await handler.Handle(new CreateAffiliationCommand(dto), CancellationToken.None);

        // Assert
        var stored = await Session.LoadAsync<AffiliationEntity>(result);
        stored.Should().NotBeNull();
        stored!.ShortName.Should().Be("MIT");
        stored.RorUrl.Should().Be("https://ror.org/042nb2s44");
    }

    #endregion

    #region UpdateAffiliation Tests

    [Fact]
    public async Task UpdateAffiliation_WithValidData_ShouldUpdateFields()
    {
        // Arrange
        var createHandler = new CreateAffiliationCommandHandler(Session);
        var id = await createHandler.Handle(
            new CreateAffiliationCommand(new CreateAffiliationDto { Name = "Old Name", RorId = "ror-1" }),
            CancellationToken.None);

        var updateHandler = new UpdateAffiliationCommandHandler(Session);
        var dto = new UpdateAffiliationDto { Name = "New Name", ShortName = "NN", RorId = "ror-2" };

        // Act
        var result = await updateHandler.Handle(new UpdateAffiliationCommand(id, dto), CancellationToken.None);

        // Assert
        result.Should().Be(id);
        var updated = await Session.LoadAsync<AffiliationEntity>(id);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("New Name");
        updated.ShortName.Should().Be("NN");
        updated.RorId.Should().Be("ror-2");
    }

    [Fact]
    public async Task UpdateAffiliation_WithNonExistentId_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new UpdateAffiliationCommandHandler(Session);
        var dto = new UpdateAffiliationDto { Name = "Any" };

        // Act
        var act = () => handler.Handle(new UpdateAffiliationCommand(Guid.NewGuid(), dto), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region DeleteAffiliation Tests

    [Fact]
    public async Task DeleteAffiliation_WithExistingAffiliation_ShouldRemoveFromStore()
    {
        // Arrange
        var createHandler = new CreateAffiliationCommandHandler(Session);
        var id = await createHandler.Handle(
            new CreateAffiliationCommand(new CreateAffiliationDto { Name = "To Delete", RorId = "ror-delete" }),
            CancellationToken.None);

        var deleteHandler = new DeleteAffiliationCommandHandler(Session);

        // Act
        await deleteHandler.Handle(new DeleteAffiliationCommand(id), CancellationToken.None);

        // Assert
        var deleted = await Session.LoadAsync<AffiliationEntity>(id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAffiliation_WithNonExistentId_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new DeleteAffiliationCommandHandler(Session);

        // Act
        var act = () => handler.Handle(new DeleteAffiliationCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region GetAffiliationById Tests

    [Fact]
    public async Task GetAffiliationById_WithExistingAffiliation_ShouldReturnMappedResult()
    {
        // Arrange
        var createHandler = new CreateAffiliationCommandHandler(Session);
        var id = await createHandler.Handle(
            new CreateAffiliationCommand(new CreateAffiliationDto { Name = "Query Affiliation", RorId = "ror-query" }),
            CancellationToken.None);

        var queryHandler = new GetAffiliationByIdQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(new GetAffiliationByIdQuery(id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Name.Should().Be("Query Affiliation");
        result.RorId.Should().Be("ror-query");
    }

    [Fact]
    public async Task GetAffiliationById_WithNonExistentId_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new GetAffiliationByIdQueryHandler(Session, Mapper);

        // Act
        var act = () => handler.Handle(new GetAffiliationByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region GetAffiliations Tests

    [Fact]
    public async Task GetAffiliations_WithNoFilter_ShouldReturnAllAffiliations()
    {
        // Arrange
        var createHandler = new CreateAffiliationCommandHandler(Session);
        await createHandler.Handle(new CreateAffiliationCommand(new CreateAffiliationDto { Name = "Affiliation A", RorId = "ror-a" }), CancellationToken.None);
        await createHandler.Handle(new CreateAffiliationCommand(new CreateAffiliationDto { Name = "Affiliation B", RorId = "ror-b" }), CancellationToken.None);
        await createHandler.Handle(new CreateAffiliationCommand(new CreateAffiliationDto { Name = "Affiliation C", RorId = "ror-c" }), CancellationToken.None);

        var queryHandler = new GetAffiliationsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(new GetAffiliationsQuery(new PaginationRequest(1, 10), null), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAffiliations_WithNameFilter_ShouldReturnMatchingAffiliations()
    {
        // Arrange
        var createHandler = new CreateAffiliationCommandHandler(Session);
        await createHandler.Handle(new CreateAffiliationCommand(new CreateAffiliationDto { Name = "MIT", RorId = "ror-mit" }), CancellationToken.None);
        await createHandler.Handle(new CreateAffiliationCommand(new CreateAffiliationDto { Name = "Stanford University", RorId = "ror-stanford" }), CancellationToken.None);
        await createHandler.Handle(new CreateAffiliationCommand(new CreateAffiliationDto { Name = "MIT Lincoln Laboratory", RorId = "ror-mitll" }), CancellationToken.None);

        var queryHandler = new GetAffiliationsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(new GetAffiliationsQuery(new PaginationRequest(1, 10), "MIT"), CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(a => a.Name!.ToLower().Should().Contain("mit"));
    }

    [Fact]
    public async Task GetAffiliations_WithWhitespaceNameFilter_ShouldReturnAllAffiliations()
    {
        // Arrange
        var createHandler = new CreateAffiliationCommandHandler(Session);
        await createHandler.Handle(new CreateAffiliationCommand(new CreateAffiliationDto { Name = "Some University", RorId = "ror-some" }), CancellationToken.None);

        var queryHandler = new GetAffiliationsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(new GetAffiliationsQuery(new PaginationRequest(1, 10), "   "), CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAffiliations_WithPagination_ShouldReturnPagedResults()
    {
        // Arrange
        var createHandler = new CreateAffiliationCommandHandler(Session);
        for (var i = 1; i <= 5; i++)
            await createHandler.Handle(new CreateAffiliationCommand(new CreateAffiliationDto { Name = $"Affiliation {i}", RorId = $"ror-{i}" }), CancellationToken.None);

        var queryHandler = new GetAffiliationsQueryHandler(Session, Mapper);

        // Act
        var result = await queryHandler.Handle(new GetAffiliationsQuery(new PaginationRequest(1, 2), null), CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Paging.Should().NotBeNull();
    }

    #endregion
}
