using Lab.Application.Features.PaperAuthor.Commands.DeletePaperAuthor;
using Lab.Application.Features.PaperAuthor.Queries.GetPaperAuthorById;
using Lab.Application.Features.PaperAuthor.Queries.GetPaperAuthors;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Application.Services;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;

namespace Lab.Application.Tests.Features.PaperAuthor;

public class PaperAuthorIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "paper_author_tests";

    private sealed class TestManagementApiService : IManagementApiService
    {
        public Task<ManagementProjectInfo?> GetProjectByIdAsync(Guid projectId, CancellationToken cancellationToken = default) => Task.FromResult<ManagementProjectInfo?>(null);
        public Task<List<ManagementProjectInfo>> GetProjectsAsync(string? name = null, string? code = null, int pageNumber = 1, int pageSize = 1000, CancellationToken cancellationToken = default) => Task.FromResult(new List<ManagementProjectInfo>());
        public Task<List<ManagementProjectInfo>> GetProjectsByIdsAsync(IEnumerable<Guid> projectIds, CancellationToken cancellationToken = default) => Task.FromResult(new List<ManagementProjectInfo>());
        public Task<Guid?> CreateSubProjectAsync(Guid projectId, Guid paperId, string? name = "", CancellationToken cancellationToken = default) => Task.FromResult<Guid?>(null);
        public Task<bool> AddSubProjectMembersAsync(Guid subProjectId, IEnumerable<(Guid UserId, string GroupName)> members, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<(Guid SubProjectId, Guid MemberId, Guid ProjectId)?> GetMemberByPaperIdAsync(Guid paperId, Guid userId, CancellationToken cancellationToken = default) => Task.FromResult<(Guid SubProjectId, Guid MemberId, Guid ProjectId)?>(null);
        public Task<ManagementMemberInfo?> GetMemberByIdAsync(Guid memberId, CancellationToken cancellationToken = default) => Task.FromResult<ManagementMemberInfo?>(null);
        public Task<List<SubProjectMemberInfo>> GetSubProjectMembersByPaperIdAsync(Guid paperId, CancellationToken cancellationToken = default) => Task.FromResult(new List<SubProjectMemberInfo>());
        public Task<Dictionary<Guid, Guid>> GetUserIdsByMemberIdsAsync(Guid paperId, IEnumerable<Guid> memberIds, CancellationToken cancellationToken = default) => Task.FromResult(new Dictionary<Guid, Guid>());
        public Task<string?> GetMyProjectRoleAsync(Guid projectId, CancellationToken cancellationToken = default) => Task.FromResult<string?>(null);
        public Task<List<Guid>?> DeleteProjectPaperByBankIdAsync(Guid paperBankId, CancellationToken cancellationToken = default) => Task.FromResult<List<Guid>?>(null);
        public Task<bool> AddProjectConferenceJournalsAsync(Guid projectId, Guid journalId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<List<Guid>?> RemoveConferenceJournalFromProjectAsync(Guid journalId, CancellationToken cancellationToken = default) => Task.FromResult<List<Guid>?>(null);
        public Task<ManagementUserAffiliationInfo?> GetUserAffiliationByIdAsync(Guid userAffiliationId, CancellationToken cancellationToken = default) => Task.FromResult<ManagementUserAffiliationInfo?>(null);
        public Task<ManagementUserAffiliationInfo?> GetUserAffiliationByUserIdAndAffiliationIdAsync(Guid userId, Guid affiliationId, CancellationToken cancellationToken = default) => Task.FromResult<ManagementUserAffiliationInfo?>(null);
    }

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

        var handler = new GetPaperAuthorByIdQueryHandler(Session, Mapper, new TestManagementApiService());
        var result = await handler.Handle(new GetPaperAuthorByIdQuery(author.Id), CancellationToken.None);

        result.PaperAuthor.Should().NotBeNull();
        result.PaperAuthor.Name.Should().Be("John");
        result.PaperAuthor.AuthorRoleName.Should().Be("Lead");
    }

    [Fact]
    public async Task GetPaperAuthorById_WithNonExistent_ShouldThrowNotFoundException()
    {
        var handler = new GetPaperAuthorByIdQueryHandler(Session, Mapper, new TestManagementApiService());
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

        var handler = new GetPaperAuthorsQueryHandler(Session, Mapper, new TestManagementApiService());
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

        var handler = new GetPaperAuthorsQueryHandler(Session, Mapper, new TestManagementApiService());
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

        var handler = new GetPaperAuthorsQueryHandler(Session, Mapper, new TestManagementApiService());
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

        var handler = new GetPaperAuthorsQueryHandler(Session, Mapper, new TestManagementApiService());
        var result = await handler.Handle(
            new GetPaperAuthorsQuery(new GetPaperAuthorsFilter { RoleName = "corresponding" }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    #endregion
}
