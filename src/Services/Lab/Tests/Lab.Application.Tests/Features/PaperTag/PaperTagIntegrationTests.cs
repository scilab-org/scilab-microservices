using Lab.Application.Features.PaperTag.Commands.AddTagToPaper;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;

namespace Lab.Application.Tests.Features.PaperTag;

public class PaperTagIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "paper_tag_tests";

    [Fact]
    public async Task AddTagToPaper_WithExistingPaper_ShouldReturnPaperId()
    {
        // Arrange — seed a paper entity directly
        var paper = PaperEntity.Create(Guid.NewGuid(), "Test Paper");
        Session.Store(paper);
        await Session.SaveChangesAsync();

        var handler = new AddTagToPaperCommandHandler(Session);
        var command = new AddTagToPaperCommand(paper.Id, new List<Guid> { Guid.NewGuid() });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(paper.Id);
    }

    [Fact]
    public async Task AddTagToPaper_WithNonExistentPaper_ShouldThrowNotFoundException()
    {
        // Arrange
        var handler = new AddTagToPaperCommandHandler(Session);
        var command = new AddTagToPaperCommand(Guid.NewGuid(), new List<Guid> { Guid.NewGuid() });

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
