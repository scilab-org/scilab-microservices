using Lab.Application.Features.Comment.Commands.UpdateComment;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;

namespace Lab.Application.Tests.Features.Comment;

public class UpdateCommentIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "comment_update_tests";

    private async Task<CommentEntity> SeedCommentAsync(string content, string userName, Guid? sectionId = null)
    {
        var comment = CommentEntity.Create(Guid.NewGuid(), sectionId ?? Guid.NewGuid(), content, userName);
        Session.Store(comment);
        await Session.SaveChangesAsync();
        return comment;
    }

    [Fact]
    public async Task UpdateComment_WithMatchingIdAndUsername_ShouldUpdateAndReturnId()
    {
        var comment = await SeedCommentAsync("Original content", "user1");

        var handler = new UpdateTemplateCommandCommandHandler(Session);
        var result = await handler.Handle(
            new UpdateCommentCommand(comment.Id, "Updated content", "user1"), CancellationToken.None);

        result.Should().Be(comment.Id);
        var updated = await Session.LoadAsync<CommentEntity>(comment.Id);
        updated!.Content.Should().Be("Updated content");
    }

    [Fact]
    public async Task UpdateComment_WithWrongUsername_ShouldThrowNotFoundException()
    {
        var comment = await SeedCommentAsync("Some content", "user1");

        var handler = new UpdateTemplateCommandCommandHandler(Session);
        var act = () => handler.Handle(
            new UpdateCommentCommand(comment.Id, "New content", "other_user"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateComment_WithNonExistentId_ShouldThrowNotFoundException()
    {
        var handler = new UpdateTemplateCommandCommandHandler(Session);
        var act = () => handler.Handle(
            new UpdateCommentCommand(Guid.NewGuid(), "Some content", "user1"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
