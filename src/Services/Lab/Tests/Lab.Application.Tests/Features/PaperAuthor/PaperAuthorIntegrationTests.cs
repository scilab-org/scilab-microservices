using Lab.Application.Features.PaperAuthor.Commands.DeletePaperAuthor;
using Lab.Application.Features.PaperAuthor.Queries.GetPaperAuthorById;
using Lab.Application.Features.PaperAuthor.Queries.GetPaperAuthors;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;

namespace Lab.Application.Tests.Features.PaperAuthor;

public class PaperAuthorIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "paper_author_tests";

    private PaperAuthorEntity SeedAuthor(Guid? paperId = null, Guid? roleId = null, string name = "Author")
    {
        var entity = PaperAuthorEntity.Create(
            Guid.NewGuid(), name, null, $"{name.ToLower()}@test.com",
            paperId ?? Guid.NewGuid(), roleId ?? Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), "University");
        Session.Store(entity);
        return entity;
    }

    #region DeletePaperAuthor

    [Fact]
    public async Task DeletePaperAuthor_WithExisting_ShouldRemoveFromStore()
    {
        var author = SeedAuthor();
        await Session.SaveChangesAsync();

        var handler = new DeletePaperAuthorCommandHandler(Session);
        await handler.Handle(new DeletePaperAuthorCommand(author.Id), CancellationToken.None);

        var deleted = await Session.LoadAsync<PaperAuthorEntity>(author.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeletePaperAuthor_WithNonExistentId_ShouldThrowNotFoundException()
    {
        var handler = new DeletePaperAuthorCommandHandler(Session);
        var act = () => handler.Handle(new DeletePaperAuthorCommand(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetPaperAuthorById

    [Fact]
    public async Task GetPaperAuthorById_WithExisting_ShouldReturnMappedResult()
    {
        var roleId = Guid.NewGuid();
        var role = new AuthorRoleEntity { Id = roleId, Name = "Lead", Description = "Lead author" };
        Session.Store(role);
        var author = SeedAuthor(roleId: roleId, name: "John");
        await Session.SaveChangesAsync();

        var handler = new GetPaperAuthorByIdQueryHandler(Session, Mapper);
        var result = await handler.Handle(new GetPaperAuthorByIdQuery(author.Id), CancellationToken.None);

        result.PaperAuthor.Should().NotBeNull();
        result.PaperAuthor.Name.Should().Be("John");
        result.PaperAuthor.AuthorRoleName.Should().Be("Lead");
    }

    [Fact]
    public async Task GetPaperAuthorById_WithNonExistent_ShouldThrowNotFoundException()
    {
        var handler = new GetPaperAuthorByIdQueryHandler(Session, Mapper);
        var act = () => handler.Handle(new GetPaperAuthorByIdQuery(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetPaperAuthors

    [Fact]
    public async Task GetPaperAuthors_WithNoFilter_ShouldReturnAll()
    {
        SeedAuthor(name: "Alice"); SeedAuthor(name: "Bob");
        await Session.SaveChangesAsync();

        var handler = new GetPaperAuthorsQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperAuthorsQuery(new GetPaperAuthorsFilter(), new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPaperAuthors_WithNameFilter_ShouldReturnMatching()
    {
        SeedAuthor(name: "Alice Smith"); SeedAuthor(name: "Bob Jones");
        await Session.SaveChangesAsync();

        var handler = new GetPaperAuthorsQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperAuthorsQuery(new GetPaperAuthorsFilter { Name = "alice" }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPaperAuthors_WithPaperIdFilter_ShouldReturnMatching()
    {
        var paperId = Guid.NewGuid();
        SeedAuthor(paperId: paperId, name: "Author1");
        SeedAuthor(paperId: paperId, name: "Author2");
        SeedAuthor(name: "Other");
        await Session.SaveChangesAsync();

        var handler = new GetPaperAuthorsQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperAuthorsQuery(new GetPaperAuthorsFilter { PaperId = paperId }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPaperAuthors_WithRoleNameFilter_ShouldReturnMatching()
    {
        var roleId = Guid.NewGuid();
        Session.Store(new AuthorRoleEntity { Id = roleId, Name = "Corresponding Author" });
        SeedAuthor(roleId: roleId, name: "Filtered");
        SeedAuthor(name: "Unfiltered");
        await Session.SaveChangesAsync();

        var handler = new GetPaperAuthorsQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperAuthorsQuery(new GetPaperAuthorsFilter { RoleName = "corresponding" }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    #endregion
}
