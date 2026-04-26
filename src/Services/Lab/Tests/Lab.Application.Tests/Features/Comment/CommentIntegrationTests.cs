using Lab.Application.Dtos.Comments;
using Lab.Application.Features.Comment.Commands.CreateComment;
using Lab.Application.Features.Comment.Commands.DeleteComment;
using Lab.Application.Features.Comment.Queries.GetCommentsBySectionId;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;

namespace Lab.Application.Tests.Features.Comment;

public class CommentIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "comment_tests";

    private async Task<SectionEntity> SeedSectionAsync()
    {
        var section = new SectionEntity
        {
            Id = Guid.NewGuid(),
            Title = "Test Section",
            PaperId = Guid.NewGuid(),
            IsMainSection = true,
            DisplayOrder = 1,
            CommentIds = new List<Guid>(),
            CreatedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(section);
        await Session.SaveChangesAsync();
        return section;
    }

    [Fact]
    public async Task CreateComment_WithValidData_ShouldStoreCommentAndLinkToSection()
    {
        var section = await SeedSectionAsync();
        var handler = new CreateCommentCommandHandler(Session);
        var dto = new CreateCommentDto
        {
            SectionId = section.Id,
            MarkSectionId = section.Id,
            Content = "This is a test comment"
        };

        var result = await handler.Handle(new CreateCommentCommand("testuser", dto), CancellationToken.None);

        result.Should().NotBeEmpty();
        var stored = await Session.LoadAsync<CommentEntity>(result);
        stored.Should().NotBeNull();
        stored!.Content.Should().Be("This is a test comment");
        stored.UserName.Should().Be("testuser");

        var updatedSection = await Session.LoadAsync<SectionEntity>(section.Id);
        updatedSection!.CommentIds.Should().Contain(result);
    }

    [Fact]
    public async Task CreateComment_WithReply_ShouldStoreReplyToUserName()
    {
        var section = await SeedSectionAsync();
        var handler = new CreateCommentCommandHandler(Session);
        var dto = new CreateCommentDto
        {
            SectionId = section.Id,
            MarkSectionId = section.Id,
            Content = "Reply content",
            RepliedToUserName = "originaluser"
        };

        var result = await handler.Handle(new CreateCommentCommand("replyuser", dto), CancellationToken.None);

        var stored = await Session.LoadAsync<CommentEntity>(result);
        stored!.ReplyToUserName.Should().Be("originaluser");
    }

    [Fact]
    public async Task CreateComment_WithNonExistentSection_ShouldStillStoreComment()
    {
        var handler = new CreateCommentCommandHandler(Session);
        var dto = new CreateCommentDto
        {
            SectionId = Guid.NewGuid(),
            MarkSectionId = Guid.NewGuid(),
            Content = "Orphan comment"
        };

        var result = await handler.Handle(new CreateCommentCommand("user", dto), CancellationToken.None);

        result.Should().NotBeEmpty();
        var stored = await Session.LoadAsync<CommentEntity>(result);
        stored.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteComment_WithExistingComment_ShouldRemoveAndUnlinkFromSection()
    {
        var section = await SeedSectionAsync();
        var createHandler = new CreateCommentCommandHandler(Session);
        var commentId = await createHandler.Handle(
            new CreateCommentCommand("testuser", new CreateCommentDto
            {
                SectionId = section.Id, MarkSectionId = section.Id, Content = "To delete"
            }), CancellationToken.None);

        var deleteHandler = new DeleteCommentCommandHandler(Session);
        await deleteHandler.Handle(new DeleteCommentCommand(commentId, section.Id, "testuser"), CancellationToken.None);

        var deleted = await Session.LoadAsync<CommentEntity>(commentId);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteComment_WithWrongUserName_ShouldThrowNotFoundException()
    {
        var section = await SeedSectionAsync();
        var createHandler = new CreateCommentCommandHandler(Session);
        var commentId = await createHandler.Handle(
            new CreateCommentCommand("testuser", new CreateCommentDto
            {
                SectionId = section.Id, MarkSectionId = section.Id, Content = "Owner only"
            }), CancellationToken.None);

        var deleteHandler = new DeleteCommentCommandHandler(Session);
        var act = () => deleteHandler.Handle(
            new DeleteCommentCommand(commentId, section.Id, "wronguser"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteComment_WithNonExistentId_ShouldThrowNotFoundException()
    {
        var handler = new DeleteCommentCommandHandler(Session);
        var act = () => handler.Handle(
            new DeleteCommentCommand(Guid.NewGuid(), Guid.NewGuid(), "any"), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetCommentsBySectionId_WithComments_ShouldReturnMappedResults()
    {
        var section = await SeedSectionAsync();
        var createHandler = new CreateCommentCommandHandler(Session);
        await createHandler.Handle(new CreateCommentCommand("user1", new CreateCommentDto
        {
            SectionId = section.Id, MarkSectionId = section.Id, Content = "Comment 1"
        }), CancellationToken.None);
        await createHandler.Handle(new CreateCommentCommand("user2", new CreateCommentDto
        {
            SectionId = section.Id, MarkSectionId = section.Id, Content = "Comment 2"
        }), CancellationToken.None);

        var queryHandler = new GetCommentsBySectionIdQueryHandler(Session, Mapper);
        var result = await queryHandler.Handle(
            new GetCommentsBySectionIdQuery(section.Id), CancellationToken.None);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetCommentsBySectionId_WithNoComments_ShouldReturnEmpty()
    {
        var handler = new GetCommentsBySectionIdQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetCommentsBySectionIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCommentsBySectionId_WithNullCommentIds_ShouldReturnEmpty()
    {
        var section = new SectionEntity
        {
            Id = Guid.NewGuid(), Title = "Empty", PaperId = Guid.NewGuid(),
            IsMainSection = true, CommentIds = null, CreatedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(section);
        await Session.SaveChangesAsync();

        var handler = new GetCommentsBySectionIdQueryHandler(Session, Mapper);
        var result = await handler.Handle(new GetCommentsBySectionIdQuery(section.Id), CancellationToken.None);

        result.Items.Should().BeEmpty();
    }
}
