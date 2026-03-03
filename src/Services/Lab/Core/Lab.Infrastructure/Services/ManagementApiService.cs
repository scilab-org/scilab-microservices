using System.Net.Http.Json;
using Common.Models.Reponses;
using Lab.Application.Services;
using Lab.Infrastructure.ApiClients;

namespace Lab.Infrastructure.Services;

public sealed class ManagementApiService(IManagementServiceApi managementServiceApi) : IManagementApiService
{
    public async Task<Guid?> CreateSubProjectAsync(
        Guid projectId,
        Guid paperId,
        string? name,
        CancellationToken cancellationToken = default)
    {
        var response = await managementServiceApi.CreateSubProjectAsync(
            projectId,
            new CreateSubProjectRequest { PaperId = paperId, Name = name });

        if (!response.IsSuccessStatusCode)
            return null;

        var body = await response.Content.ReadFromJsonAsync<ApiCreatedResponse<Guid>>(
            cancellationToken: cancellationToken);

        return body?.Value;
    }
}