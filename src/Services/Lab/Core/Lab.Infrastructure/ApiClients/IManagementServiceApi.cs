using Refit;

namespace Lab.Infrastructure.ApiClients;

public interface IManagementServiceApi
{
    /// <summary>
    /// POST /projects/{projectId}/sub-projects — creates a sub-project with a paper.
    /// </summary>
    [Post("/projects/{projectId}/sub-projects")]
    Task<HttpResponseMessage> CreateSubProjectAsync(
        [AliasAs("projectId")] Guid projectId,
        [Body] CreateSubProjectRequest body);
}

public class CreateSubProjectRequest
{
    public Guid PaperId { get; set; }
    public string? Name { get; set; }
}