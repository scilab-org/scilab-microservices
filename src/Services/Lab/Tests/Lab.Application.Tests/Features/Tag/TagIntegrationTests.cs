using Lab.Application.Features.Tag.Commands.CreateTag;
using Lab.Application.Features.Tag.Commands.DeleteTag;
using Lab.Application.Features.Tag.Commands.UpdateTag;
using Lab.Application.Features.Tag.Queries.GetTagById;
using Lab.Application.Features.Tag.Queries.GetTags;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Tests.Features.Tag;

public class TagIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "tag_tests";

    #region CreateTag Tests

    [Fact]
    public async Task CreateTag_WithValidName_ShouldStoreAndReturnId()
    {
        // Arrange
        var handler = new CreateTagCommandHandler(Session);
        var command = new CreateTagCommand("machine learning");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();

        var stored = await Session.LoadAsync<KeywordEntity>(result);
        stored.Should().NotBeNull();
        stored!.Name.Should().Be("machine learning");
    }

    [Fact]
    public async Task CreateTag_ShouldNormalizeName_ToLowerCaseTrimmed()
    {
        // Arrange
        var handler = new CreateTagCommandHandler(Session);
        var command = new CreateTagCommand("  Deep Learning  ");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var stored = await Session.LoadAsync<KeywordEntity>(result);
        stored.Should().NotBeNull();
        stored!.Name.Should().Be("deep learning");
    }

    [Fact]
    public async Task CreateTag_WithDuplicateName_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new CreateTagCommandHandler(Session);
        await handler.Handle(new CreateTagCommand("ai"), CancellationToken.None);

        // Act
        var act = () => handler.Handle(new CreateTagCommand("ai"), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task CreateTag_WithDuplicateNameDifferentCase_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new CreateTagCommandHandler(Session);
        await handler.Handle(new CreateTagCommand("neural network"), CancellationToken.None);

        // Act — same name in uppercase
        var act = () => handler.Handle(new CreateTagCommand("  NEURAL NETWORK  "), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region UpdateTag Tests

    [Fact]
    public async Task UpdateTag_WithValidData_ShouldUpdateName()
    {
        // Arrange
        var createHandler = new CreateTagCommandHandler(Session);
        var id = await createHandler.Handle(new CreateTagCommand("old name"), CancellationToken.None);

        var updateHandler = new UpdateTagCommandHandler(Session);
        var command = new UpdateTagCommand(id, "new name");

        // Act
        var result = await updateHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(id);

        var updated = await Session.LoadAsync<KeywordEntity>(id);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("new name");
    }

    [Fact]
    public async Task UpdateTag_WithNonExistentId_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new UpdateTagCommandHandler(Session);
        var command = new UpdateTagCommand(Guid.NewGuid(), "any name");

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task UpdateTag_WithDuplicateName_ShouldThrowClientValidationException()
    {
        // Arrange
        var createHandler = new CreateTagCommandHandler(Session);
        await createHandler.Handle(new CreateTagCommand("existing tag"), CancellationToken.None);
        var tagToUpdateId = await createHandler.Handle(new CreateTagCommand("other tag"), CancellationToken.None);

        var updateHandler = new UpdateTagCommandHandler(Session);
        var command = new UpdateTagCommand(tagToUpdateId, "existing tag");

        // Act
        var act = () => updateHandler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region DeleteTag Tests

    [Fact]
    public async Task DeleteTag_WithExistingTag_ShouldRemoveFromStore()
    {
        // Arrange
        var createHandler = new CreateTagCommandHandler(Session);
        var id = await createHandler.Handle(new CreateTagCommand("to delete"), CancellationToken.None);

        var deleteHandler = new DeleteTagCommandHandler(Session);
        var command = new DeleteTagCommand(id);

        // Act
        await deleteHandler.Handle(command, CancellationToken.None);

        // Assert
        // With soft deletes enabled, Query() filters out soft-deleted records
        var deletedTag = await Session.Query<KeywordEntity>().FirstOrDefaultAsync(x => x.Id == id);
        deletedTag.Should().BeNull();
    }

    [Fact]
    public async Task DeleteTag_WithNonExistentId_ShouldThrowClientValidationException()
    {
        // Arrange
        var handler = new DeleteTagCommandHandler(Session);
        var command = new DeleteTagCommand(Guid.NewGuid());

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region GetTagById Tests

    [Fact]
    public async Task GetTagById_WithExistingTag_ShouldReturnMappedResult()
    {
        // Arrange
        var createHandler = new CreateTagCommandHandler(Session);
        var id = await createHandler.Handle(new CreateTagCommand("query tag"), CancellationToken.None);

        var queryHandler = new GetTagByIdQueryHandler(Session, Mapper);
        var query = new GetTagByIdQuery(id);

        // Act
        var result = await queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Tag.Should().NotBeNull();
        result.Tag.Id.Should().Be(id);
        result.Tag.Name.Should().Be("query tag");
    }

    [Fact]
    public async Task GetTagById_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        var handler = new GetTagByIdQueryHandler(Session, Mapper);
        var query = new GetTagByIdQuery(Guid.NewGuid());

        // Act
        var act = () => handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetTags Tests

    [Fact]
    public async Task GetTags_WithNoFilter_ShouldReturnAllTags()
    {
        // Arrange
        var createHandler = new CreateTagCommandHandler(Session);
        await createHandler.Handle(new CreateTagCommand("tag alpha"), CancellationToken.None);
        await createHandler.Handle(new CreateTagCommand("tag beta"), CancellationToken.None);
        await createHandler.Handle(new CreateTagCommand("tag gamma"), CancellationToken.None);

        var queryHandler = new GetTagsQueryHandler(Session, Mapper);
        var query = new GetTagsQuery(new GetTagsFilter(), new PaginationRequest(1, 10));

        // Act
        var result = await queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetTags_WithNameFilter_ShouldReturnMatchingTags()
    {
        // Arrange
        var createHandler = new CreateTagCommandHandler(Session);
        await createHandler.Handle(new CreateTagCommand("data science"), CancellationToken.None);
        await createHandler.Handle(new CreateTagCommand("data mining"), CancellationToken.None);
        await createHandler.Handle(new CreateTagCommand("robotics"), CancellationToken.None);

        var queryHandler = new GetTagsQueryHandler(Session, Mapper);
        var filter = new GetTagsFilter { Name = "data" };
        var query = new GetTagsQuery(filter, new PaginationRequest(1, 10));

        // Act
        var result = await queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(t => t.Name.Should().Contain("data"));
    }

    [Fact]
    public async Task GetTags_WithPagination_ShouldReturnPagedResults()
    {
        // Arrange
        var createHandler = new CreateTagCommandHandler(Session);
        for (var i = 1; i <= 5; i++)
            await createHandler.Handle(new CreateTagCommand($"paged tag {i}"), CancellationToken.None);

        var queryHandler = new GetTagsQueryHandler(Session, Mapper);
        var query = new GetTagsQuery(new GetTagsFilter(), new PaginationRequest(1, 2));

        // Act
        var result = await queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Paging.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTags_WithIsDeletedFilter_ShouldReturnDeletedTags()
    {
        // Arrange
        var createHandler = new CreateTagCommandHandler(Session);
        var id1 = await createHandler.Handle(new CreateTagCommand("active tag"), CancellationToken.None);
        var id2 = await createHandler.Handle(new CreateTagCommand("deleted tag"), CancellationToken.None);

        var deleteHandler = new DeleteTagCommandHandler(Session);
        await deleteHandler.Handle(new DeleteTagCommand(id2), CancellationToken.None);

        var queryHandler = new GetTagsQueryHandler(Session, Mapper);
        var filter = new GetTagsFilter { IsDeleted = true };
        var query = new GetTagsQuery(filter, new PaginationRequest(1, 10));

        // Act
        var result = await queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().Id.Should().Be(id2);
    }

    #endregion
}
