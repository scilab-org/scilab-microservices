using Lab.Application.Dtos.GapTypes;
using Lab.Application.Features.GapType.Commands.CreateGapType;
using Lab.Application.Features.GapType.Commands.DeleteGapType;
using Lab.Application.Features.GapType.Commands.UpdateGapType;
using Lab.Application.Features.GapType.Queries.GetGapTypeById;
using Lab.Application.Features.GapType.Queries.GetGapTypes;
using Lab.Application.Models.Results;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;

namespace Lab.Application.Tests.Features.GapType;

public class GapTypeIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "gap_type_tests";

    [Fact]
    public async Task CreateGapType_WithValidName_ShouldStoreAndReturnId()
    {
        var handler = new CreateGapTypeCommandHandler(Session);
        var result = await handler.Handle(new CreateGapTypeCommand(new CreateGapTypeDto { Name = "Empirical Gap" }), CancellationToken.None);

        result.Should().NotBeEmpty();
        var stored = await Session.LoadAsync<GapTypeEntity>(result);
        stored.Should().NotBeNull();
        stored!.Name.Should().Be("Empirical Gap");
    }

    [Fact]
    public async Task CreateGapType_ShouldTrimName()
    {
        var handler = new CreateGapTypeCommandHandler(Session);
        var result = await handler.Handle(new CreateGapTypeCommand(new CreateGapTypeDto { Name = "  Theoretical Gap  " }), CancellationToken.None);

        var stored = await Session.LoadAsync<GapTypeEntity>(result);
        stored!.Name.Should().Be("Theoretical Gap");
    }

    [Fact]
    public async Task UpdateGapType_WithValidData_ShouldUpdateName()
    {
        var createHandler = new CreateGapTypeCommandHandler(Session);
        var id = await createHandler.Handle(new CreateGapTypeCommand(new CreateGapTypeDto { Name = "Old Gap" }), CancellationToken.None);

        var updateHandler = new UpdateGapTypeCommandHandler(Session);
        var result = await updateHandler.Handle(new UpdateGapTypeCommand(id, new UpdateGapTypeDto { Name = "New Gap" }), CancellationToken.None);

        result.Should().Be(id);
        var updated = await Session.LoadAsync<GapTypeEntity>(id);
        updated!.Name.Should().Be("New Gap");
    }

    [Fact]
    public async Task UpdateGapType_WithNullName_ShouldKeepExistingName()
    {
        var createHandler = new CreateGapTypeCommandHandler(Session);
        var id = await createHandler.Handle(new CreateGapTypeCommand(new CreateGapTypeDto { Name = "Keep This" }), CancellationToken.None);

        var updateHandler = new UpdateGapTypeCommandHandler(Session);
        await updateHandler.Handle(new UpdateGapTypeCommand(id, new UpdateGapTypeDto { Name = null }), CancellationToken.None);

        var updated = await Session.LoadAsync<GapTypeEntity>(id);
        updated!.Name.Should().Be("Keep This");
    }

    [Fact]
    public async Task UpdateGapType_WithNonExistentId_ShouldThrowNotFoundException()
    {
        var handler = new UpdateGapTypeCommandHandler(Session);
        var act = () => handler.Handle(new UpdateGapTypeCommand(Guid.NewGuid(), new UpdateGapTypeDto { Name = "any" }), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteGapType_WithExistingEntity_ShouldRemoveFromStore()
    {
        var createHandler = new CreateGapTypeCommandHandler(Session);
        var id = await createHandler.Handle(new CreateGapTypeCommand(new CreateGapTypeDto { Name = "To Delete" }), CancellationToken.None);

        var deleteHandler = new DeleteGapTypeCommandHandler(Session);
        await deleteHandler.Handle(new DeleteGapTypeCommand(id), CancellationToken.None);

        var deleted = await Session.LoadAsync<GapTypeEntity>(id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteGapType_WithNonExistentId_ShouldThrowNotFoundException()
    {
        var handler = new DeleteGapTypeCommandHandler(Session);
        var act = () => handler.Handle(new DeleteGapTypeCommand(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetGapTypeById_WithExistingEntity_ShouldReturnMappedResult()
    {
        var createHandler = new CreateGapTypeCommandHandler(Session);
        var id = await createHandler.Handle(new CreateGapTypeCommand(new CreateGapTypeDto { Name = "Query Gap" }), CancellationToken.None);

        var queryHandler = new GetGapTypeByIdQueryHandler(Session, Mapper);
        var result = await queryHandler.Handle(new GetGapTypeByIdQuery(id), CancellationToken.None);

        result.GapType.Should().NotBeNull();
        result.GapType.Name.Should().Be("Query Gap");
    }

    [Fact]
    public async Task GetGapTypeById_WithNonExistentId_ShouldThrowNotFoundException()
    {
        var handler = new GetGapTypeByIdQueryHandler(Session, Mapper);
        var act = () => handler.Handle(new GetGapTypeByIdQuery(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetGapTypes_WithNoFilter_ShouldReturnAll()
    {
        var createHandler = new CreateGapTypeCommandHandler(Session);
        await createHandler.Handle(new CreateGapTypeCommand(new CreateGapTypeDto { Name = "Gap A" }), CancellationToken.None);
        await createHandler.Handle(new CreateGapTypeCommand(new CreateGapTypeDto { Name = "Gap B" }), CancellationToken.None);

        var queryHandler = new GetGapTypesQueryHandler(Session, Mapper);
        var result = await queryHandler.Handle(new GetGapTypesQuery(new PaginationRequest(1, 10), null), CancellationToken.None);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetGapTypes_WithNameFilter_ShouldReturnMatchingTypes()
    {
        var createHandler = new CreateGapTypeCommandHandler(Session);
        await createHandler.Handle(new CreateGapTypeCommand(new CreateGapTypeDto { Name = "Empirical Gap" }), CancellationToken.None);
        await createHandler.Handle(new CreateGapTypeCommand(new CreateGapTypeDto { Name = "Theoretical Gap" }), CancellationToken.None);

        var queryHandler = new GetGapTypesQueryHandler(Session, Mapper);
        var result = await queryHandler.Handle(new GetGapTypesQuery(new PaginationRequest(1, 10), "empirical"), CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetGapTypes_WithPagination_ShouldReturnPagedResults()
    {
        var createHandler = new CreateGapTypeCommandHandler(Session);
        for (var i = 1; i <= 5; i++)
            await createHandler.Handle(new CreateGapTypeCommand(new CreateGapTypeDto { Name = $"Paged Gap {i}" }), CancellationToken.None);

        var queryHandler = new GetGapTypesQueryHandler(Session, Mapper);
        var result = await queryHandler.Handle(new GetGapTypesQuery(new PaginationRequest(1, 2), null), CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Paging.Should().NotBeNull();
    }
}
