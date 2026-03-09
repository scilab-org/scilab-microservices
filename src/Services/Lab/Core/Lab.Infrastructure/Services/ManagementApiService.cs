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
}

/// <summary>Minimal shape to extract SubProjectId + MemberId from the combined member-by-paper response.</summary>
file sealed class MemberByPaperDto
{
    public Guid SubProjectId { get; init; }
    public Guid MemberId { get; init; }
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
