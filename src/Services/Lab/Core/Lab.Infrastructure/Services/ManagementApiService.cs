using System.Net.Http.Json;
using System.Text.Json;
using Common.Models.Reponses;
using Common.Constants;
using Lab.Application.Services;
using Lab.Infrastructure.ApiClients;

namespace Lab.Infrastructure.Services;

public sealed class ManagementApiService(IManagementServiceApi managementServiceApi) : IManagementApiService
{
    public async Task<ManagementProjectInfo?> GetProjectByIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var response = await managementServiceApi.GetProjectByIdAsync(projectId);

        if (!response.IsSuccessStatusCode)
            return null;

        var body = await response.Content.ReadFromJsonAsync<ApiGetResponse<GetProjectByIdApiResult>>(
            cancellationToken: cancellationToken);

        var project = body?.Result.Project;
        if (project is null)
            return null;

        return new ManagementProjectInfo(
            project.Id,
            project.Name,
            project.Code,
            project.Description,
            NormalizeStatus(project.Status),
            project.StartDate,
            project.EndDate,
            project.Context,
            project.Domain,
            project.Keypoint);
    }

    public async Task<List<ManagementProjectInfo>> GetProjectsAsync(
        string? name = null,
        string? code = null,
        int pageNumber = 1,
        int pageSize = 1000,
        CancellationToken cancellationToken = default)
    {
        var response = await managementServiceApi.GetProjectsAsync(name, code, pageNumber, pageSize);

        if (!response.IsSuccessStatusCode)
            return [];

        var body = await response.Content.ReadFromJsonAsync<ApiGetResponse<GetProjectsApiResult>>(
            cancellationToken: cancellationToken);

        var items = body?.Result.Items ?? [];

        return items
            .Select(project => new ManagementProjectInfo(
                project.Id,
                project.Name,
                project.Code,
                project.Description,
                NormalizeStatus(project.Status),
                project.StartDate,
                project.EndDate,
                project.Context,
                project.Domain,
                project.Keypoint))
            .ToList();
    }

    public async Task<List<ManagementProjectInfo>> GetProjectsByIdsAsync(
        IEnumerable<Guid> projectIds,
        CancellationToken cancellationToken = default)
    {
        var ids = projectIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        if (ids.Count == 0)
            return [];

        var projectTasks = ids.Select(id => GetProjectByIdAsync(id, cancellationToken));
        var projects = await Task.WhenAll(projectTasks);

        return projects
            .Where(project => project != null)
            .Cast<ManagementProjectInfo>()
            .ToList();
    }

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

    public async Task<bool> AddSubProjectMembersAsync(
        Guid subProjectId,
        IEnumerable<(Guid UserId, string GroupName)> members,
        CancellationToken cancellationToken = default)
    {
        var request = new AddSubProjectMembersRequest
        {
            Members = members
                .Where(x => x.UserId != Guid.Empty)
                .Select(x => new AddSubProjectMemberEntry
                {
                    UserId = x.UserId,
                    GroupName = string.IsNullOrWhiteSpace(x.GroupName)
                        ? AuthorizeConstants.ProjectAuthor
                        : x.GroupName
                })
                .ToList()
        };

        if (request.Members.Count == 0)
            return false;

        var response = await managementServiceApi.AddSubProjectMembersAsync(subProjectId, request);

        return response.IsSuccessStatusCode;
    }

    public async Task<string?> GetMyProjectRoleAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var response = await managementServiceApi.GetMyProjectRoleAsync(projectId);

        if (!response.IsSuccessStatusCode)
            return null;

        var body = await response.Content.ReadFromJsonAsync<ApiGetResponse<string>>(
            cancellationToken: cancellationToken);

        return body?.Result;
    }

    public async Task<(Guid SubProjectId, Guid MemberId)?> GetMemberByPaperIdAsync(
        Guid paperId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var response = await managementServiceApi.GetMemberByPaperIdAsync(paperId, userId);

        if (!response.IsSuccessStatusCode)
            return null;

        var body = await response.Content.ReadFromJsonAsync<ApiGetResponse<MemberByPaperDto>>(
            cancellationToken: cancellationToken);

        var dto = body?.Result;
        if (dto == null)
            return null;

        return (dto.SubProjectId, dto.MemberId);
    }

    public async Task<List<Guid>?> DeleteProjectPaperByBankIdAsync(
        Guid paperBankId,
        CancellationToken cancellationToken = default)
    {
        var response = await managementServiceApi.DeleteProjectPaperByBankIdAsync(paperBankId);

        // if (!response.IsSuccessStatusCode)
        //     return null;

        var body = await response.Content.ReadFromJsonAsync<ApiDeletedResponse<List<Guid>>>(
            cancellationToken: cancellationToken);

        return body?.Value;
    }

    public async Task<bool> AddProjectConferenceJournalsAsync(
        Guid projectId,
        Guid journalId,
        CancellationToken cancellationToken = default)
    {
        var body = await managementServiceApi.AddProjectConferenceJournalsAsync(
            projectId,
            journalId);

        return body.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveProjectConferenceJournalsAsync(
        Guid projectId,
        Guid journalId,
        CancellationToken cancellationToken = default)
    {
        var body = await managementServiceApi.RemoveProjectConferenceJournalsAsync(
            projectId,
            journalId);

        return body.IsSuccessStatusCode;
    }

    public async Task<List<Guid>?> RemoveConferenceJournalFromProjectAsync(
        Guid journalId,
        CancellationToken cancellationToken = default)
    {
        if (journalId == Guid.Empty)
            return [];

        var response = await managementServiceApi.RemoveConferenceJournalFromProjectAsync(journalId);

        if (!response.IsSuccessStatusCode)
            return null;

        var body = await response.Content.ReadFromJsonAsync<ApiDeletedResponse<List<Guid>>>(
            cancellationToken: cancellationToken);

        return body?.Value;
    }

    public async Task<List<SubProjectMemberInfo>> GetSubProjectMembersByPaperIdAsync(
        Guid paperId,
        CancellationToken cancellationToken = default)
    {
        // Single call — new endpoint resolves subProject from paperId and returns all its members
        var response = await managementServiceApi.GetSubProjectMembersByPaperIdAsync(paperId);
        if (!response.IsSuccessStatusCode) return [];

        var body = await response.Content
            .ReadFromJsonAsync<ApiGetResponse<SubProjectMembersPagedResult>>(cancellationToken: cancellationToken);

        return body?.Result?.Items?
            .Select(m => new SubProjectMemberInfo(
                m.MemberId, m.UserId, m.Role ?? string.Empty,
                m.Username, m.Email, m.FirstName, m.LastName))
            .ToList() ?? [];
    }

    public async Task<Dictionary<Guid, Guid>> GetUserIdsByMemberIdsAsync(
        Guid paperId,
        IEnumerable<Guid> memberIds,
        CancellationToken cancellationToken = default)
    {
        var memberIdSet = memberIds.ToHashSet();
        if (memberIdSet.Count == 0) return [];

        var allMembers = await GetSubProjectMembersByPaperIdAsync(paperId, cancellationToken);

        return allMembers
            .Where(m => memberIdSet.Contains(m.MemberId))
            .ToDictionary(m => m.MemberId, m => m.UserId);
    }

    private static string? NormalizeStatus(JsonElement status)
    {
        return status.ValueKind switch
        {
            JsonValueKind.String => status.GetString(),
            JsonValueKind.Number => status.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => null,
            JsonValueKind.Undefined => null,
            _ => status.GetRawText()
        };
    }
}

/// <summary>Minimal shape to extract SubProjectId + MemberId from the combined member-by-paper response.</summary>
file sealed class MemberByPaperDto
{
    public Guid SubProjectId { get; init; }
    public Guid MemberId { get; init; }
}

file sealed class GetProjectByIdApiResult
{
    public ProjectApiDto? Project { get; init; }
}

file sealed class GetProjectsApiResult
{
    public List<ProjectApiDto>? Items { get; init; }
}

file sealed class ProjectApiDto
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Code { get; init; }
    public string? Description { get; init; }
    public JsonElement Status { get; init; }
    public DateTimeOffset? StartDate { get; init; }
    public DateTimeOffset? EndDate { get; init; }
    public string? Context { get; init; }
    public string? Domain { get; init; }
    public string? Keypoint { get; init; }
}

// Internal DTOs for parsing Management service responses
file sealed class MemberItem
{
    public Guid    MemberId  { get; init; }
    public Guid    UserId    { get; init; }
    public string? Role      { get; init; }
    public string? Username  { get; init; }
    public string? Email     { get; init; }
    public string? FirstName { get; init; }
    public string? LastName  { get; init; }
}
file sealed class SubProjectMembersPagedResult { public List<MemberItem>? Items { get; init; } }