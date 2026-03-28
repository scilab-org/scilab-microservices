using Refit;

namespace Lab.Infrastructure.ApiClients;

public interface IManagementServiceApi
{
    /// <summary>
    /// GET /projects/{projectId} — fetches project details by id.
    /// </summary>
    [Get("/projects/{projectId}")]
    Task<HttpResponseMessage> GetProjectByIdAsync(
        [AliasAs("projectId")] Guid projectId);

    /// <summary>
    /// POST /projects/{projectId}/sub-projects — creates a sub-project with a paper.
    /// </summary>
    [Post("/projects/{projectId}/sub-projects")]
    Task<HttpResponseMessage> CreateSubProjectAsync(
        [AliasAs("projectId")] Guid projectId,
        [Body] CreateSubProjectRequest body);

    /// <summary>
    /// POST /sub-projects/{subProjectId}/members — adds members to a sub-project.
    /// </summary>
    [Post("/sub-projects/{subProjectId}/members")]
    Task<HttpResponseMessage> AddSubProjectMembersAsync(
        [AliasAs("subProjectId")] Guid subProjectId,
        [Body] AddSubProjectMembersRequest body);

    /// <summary>
    /// GET /sub-projects/papers/{paperId}/member?userId= — single call that resolves
    /// subProjectId from paperId and returns the member record in one round-trip.
    /// </summary>
    [Get("/sub-projects/papers/{paperId}/member")]
    Task<HttpResponseMessage> GetMemberByPaperIdAsync(
        [AliasAs("paperId")] Guid paperId,
        [AliasAs("userId")] Guid userId);

    /// <summary>GET /sub-projects/papers/{paperId}/members — all members of the sub-project that owns this paper.</summary>
    [Get("/sub-projects/papers/{paperId}/members")]
    Task<HttpResponseMessage> GetSubProjectMembersByPaperIdAsync(
        [AliasAs("paperId")] Guid paperId);

    /// <summary>
    /// GET /projects/{projectId}/my-role — gets current user's role in the project.
    /// </summary>
    [Get("/projects/{projectId}/my-role")]
    Task<HttpResponseMessage> GetMyProjectRoleAsync(
        [AliasAs("projectId")] Guid projectId);

    /// <summary>
    /// POST /projects/paper-bank/{paperBankId} — removes this paper id from all projects.
    /// </summary>
    [Post("/projects/paper-bank/{paperBankId}")]
    Task<HttpResponseMessage> DeleteProjectPaperByBankIdAsync(
        [AliasAs("paperBankId")] Guid paperBankId);
}

public class CreateSubProjectRequest
{
    public Guid PaperId { get; set; }
    public string? Name { get; set; }
}

public sealed class AddSubProjectMembersRequest
{
    public List<AddSubProjectMemberEntry> Members { get; set; } = [];
}

public sealed class AddSubProjectMemberEntry
{
    public Guid UserId { get; set; }
    public string GroupName { get; set; } = Common.Constants.AuthorizeConstants.ProjectAuthor;
}
