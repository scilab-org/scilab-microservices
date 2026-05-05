using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using Management.Application.Dtos.Dashboard;
using Management.Application.Dtos.Papers;
using Management.Application.Services;
using Management.Infrastructure.ApiClients;
using BuildingBlocks.Pagination;

namespace Management.Infrastructure.Services;

// Mirror of Lab.Domain.Enums.PaperStatus — kept in sync with Lab service
file enum PaperStatus
{
    Draft = 1,
    Processing = 2,
    Submited = 3,
    Released = 4,
    Sampled = 5
}

// Internal shapes matching Lab service JSON response
// Internal deserialization shapes — LabPaperItem (bank/list) and LabPaperFull (single paper entity)
[ExcludeFromCodeCoverage]
file sealed class LabPaperItem
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Authors { get; set; }
    public string? Publisher { get; set; }
    public string? Ranking { get; set; }
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public string? Url { get; set; }
    public string? FilePath { get; set; }
    public string? BibFilePath { get; set; }
    public string? ParsedText { get; set; }
    public bool? IsIngested { get; set; }
    public bool? IsAutoTagged { get; set; }
    public DateTimeOffset? PublicationDate { get; set; }
    public string? Pages { get; set; }
    public string? Number { get; set; }
    public string? Volume { get; set; }
    public Guid? ConferenceJournalId { get; set; }
    public string? ConferenceJournalName { get; set; }
    public int? ConferenceJournalType { get; set; }
    public string? ReferenceContent { get; set; }
    public List<string> Keywords { get; set; } = new();
    public int? IngestStatus { get; set; }
    public string? CreatedBy { get; set; }
}

// GET /papers/sample  =>  { "result": { "items": [...], "paging": {...} } }
[ExcludeFromCodeCoverage]
file sealed class LabGetPapersResult
{
    public List<LabPaperItem> Items { get; set; } = new();
}

[ExcludeFromCodeCoverage]
file sealed class LabGetPapersResponse
{
    public LabGetPapersResult? Result { get; set; }
}

// GET /paper-bank  =>  { "result": { "items": [...], "paging": { "totalCount": ... } } }
[ExcludeFromCodeCoverage]
file sealed class LabGetPaperBanksPaging
{
    public long TotalCount { get; set; }
}

[ExcludeFromCodeCoverage]
file sealed class LabGetPaperBanksResult
{
    public List<LabPaperItem> Items { get; set; } = new();
    public LabGetPaperBanksPaging? Paging { get; set; }
}

[ExcludeFromCodeCoverage]
file sealed class LabGetPaperBanksResponse
{
    public LabGetPaperBanksResult? Result { get; set; }
}

// GET /papers/{id}  =>  { "result": { "paper": { ...PaperDto... } } }
// PaperDto — Lab paper entity fields (title, template, context, status, etc.)
[ExcludeFromCodeCoverage]
file sealed class LabPaperFull
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Template { get; set; }
    public string? FilePath { get; set; }
    public string? Context { get; set; }
    public string? Abstract { get; set; }
    public string? ResearchGap { get; set; }
    public string? MainContribution { get; set; }
    public string? ResearchAim { get; set; }
    public string? Rule { get; set; }
    public Guid? ConferenceJournalId { get; set; }
    public string? ConferenceJournalName { get; set; }
    public int? ConferenceJournalType { get; set; }
    public DateTimeOffset? ConferenceJournalStartAt { get; set; }
    public DateTimeOffset? ConferenceJournalEndAt { get; set; }
    public Guid? SubProjectId { get; set; }
    public int? Status { get; set; }
    public int? SubmissionStatus { get; set; }
    public List<string> TagNames { get; set; } = new();
    public string? CreatedBy { get; set; }
}

[ExcludeFromCodeCoverage]
file sealed class LabGetPaperByIdResult
{
    public LabPaperFull? Paper { get; set; }
}

[ExcludeFromCodeCoverage]
file sealed class LabGetPaperByIdResponse
{
    public LabGetPaperByIdResult? Result { get; set; }
}

// GET /paper-bank/{id}  =>  { "result": { "paperBank": { ... } } }
[ExcludeFromCodeCoverage]
file sealed class LabGetPaperBankByIdResult
{
    public LabPaperItem? PaperBank { get; set; }
}

[ExcludeFromCodeCoverage]
file sealed class LabGetPaperBankByIdResponse
{
    public LabGetPaperBankByIdResult? Result { get; set; }
}

// GET /papers/{id}/sections  =>  { "result": { "items": [...] } }
[ExcludeFromCodeCoverage]
file sealed class LabSectionItem
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public float DisplayOrder { get; set; }
    public Guid? ParentSectionId { get; set; }
    public Guid PaperId { get; set; }
    public string? SectionRole { get; set; }
}

[ExcludeFromCodeCoverage]
file sealed class LabGetSectionsResult
{
    public List<LabSectionItem>? Items { get; set; }
}

[ExcludeFromCodeCoverage]
file sealed class LabGetSectionsResponse
{
    public LabGetSectionsResult? Result { get; set; }
}

[ExcludeFromCodeCoverage]
file sealed class LabPaperContributorItem
{
    public Guid Id { get; set; }
    public Guid PaperId { get; set; }
    public Guid MemberId { get; set; }
    public Guid MarkSectionId { get; set; }
    public Guid? SectionId { get; set; }
    public string? SectionRole { get; set; }
    public Guid UserId { get; set; }
}

[ExcludeFromCodeCoverage]
file sealed class LabPaperAuthorItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? OcrId { get; set; }
    public string Email { get; set; } = null!;
    public Guid PaperId { get; set; }
    public Guid AuthorRoleId { get; set; }
    public string? AuthorRoleName { get; set; }
    public string? AuthorRoleDescription { get; set; }
    public Guid MemberId { get; set; }
}

[ExcludeFromCodeCoverage]
file sealed class LabGetPaperAuthorsResult
{
    public List<LabPaperAuthorItem> Items { get; set; } = new();
}

[ExcludeFromCodeCoverage]
file sealed class LabGetPaperAuthorsResponse
{
    public LabGetPaperAuthorsResult? Result { get; set; }
}

[ExcludeFromCodeCoverage]
file sealed class LabGetPaperContributorsResult
{
    public List<LabPaperContributorItem> Items { get; set; } = new();
}

[ExcludeFromCodeCoverage]
file sealed class LabGetPaperContributorsResponse
{
    public LabGetPaperContributorsResult? Result { get; set; }
}

[ExcludeFromCodeCoverage]
file sealed class AssignedPapersPagedResult
{
    public List<PaperInfoDto>? Items { get; set; }
    public PagingResult? Paging { get; set; }
}

public sealed class LabApiService(ILabServiceApi labServiceApi) : ILabApiService
{
    #region Implementations

    public async Task<(List<PaperBankInfoDto> Items, long TotalCount)> GetAvailablePapersAsync(
        IEnumerable<Guid> existingPaperIds,
        string? title = null,
        string[]? author = null,
        string? publisher = null,
        string? @abstract = null,
        string? doi = null,
        int? status = null,
        DateTimeOffset? fromPublicationDate = null,
        DateTimeOffset? toPublicationDate = null,
        string? paperType = null,
        string? journalName = null,
        string? conferenceName = null,
        string[]? keywords = null,
        int pageNumber = 1,
        int pageSize = 1000,
        CancellationToken cancellationToken = default)
    {
        var existingSet = existingPaperIds.ToHashSet();

        var response = await labServiceApi.GetPaperBanksAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            title: title,
            author: author,
            publisher: publisher,
            @abstract: @abstract,
            doi: doi,
            status: status,
            fromPublicationDate: fromPublicationDate,
            toPublicationDate: toPublicationDate,
            paperType: paperType,
            journalName: journalName,
            conferenceName: conferenceName,
            keywords: keywords,
            existingPaperIds: existingSet.Count > 0 ? existingSet.ToArray() : null);

        if (!response.IsSuccessStatusCode)
            return (new List<PaperBankInfoDto>(), 0);

        var body = await response.Content.ReadFromJsonAsync<LabGetPaperBanksResponse>(
            cancellationToken: cancellationToken);

        var allItems = body?.Result?.Items ?? new List<LabPaperItem>();

        var filtered = allItems
            .Select(p => p.MapToDto())
            .ToList();

        var totalCount = body?.Result?.Paging?.TotalCount ?? filtered.Count;

        return (filtered, totalCount);
    }

    public async Task<PaperInfoDto?> GetPaperByIdAsync(
        Guid paperId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await labServiceApi.GetPaperByIdAsync(paperId);
            if (!response.IsSuccessStatusCode) return null;

            var body = await response.Content.ReadFromJsonAsync<LabGetPaperByIdResponse>(
                cancellationToken: cancellationToken);

            return body?.Result?.Paper?.MapToPaperDto();
        }
        catch
        {
            return null;
        }
    }

    public async Task<PaperBankInfoDto?> GetPaperBankByIdAsync(
        Guid paperId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await labServiceApi.GetPaperBankByIdAsync(paperId);
            if (!response.IsSuccessStatusCode) return null;

            var body = await response.Content.ReadFromJsonAsync<LabGetPaperBankByIdResponse>(
                cancellationToken: cancellationToken);

            return body?.Result?.PaperBank?.MapToDto();
        }
        catch
        {
            return null;
        }
    }
    public async Task<List<PaperInfoDto>> GetPapersByIdsAsync(
        IEnumerable<Guid> paperIds,
        CancellationToken cancellationToken = default)
    {
        var idSet = paperIds.ToHashSet();
        if (idSet.Count == 0) return new List<PaperInfoDto>();

        var result = new List<PaperInfoDto>();

        foreach (var paperId in idSet)
        {
            try
            {
                var response = await labServiceApi.GetPaperByIdAsync(paperId);
                if (!response.IsSuccessStatusCode) continue;

                var body = await response.Content.ReadFromJsonAsync<LabGetPaperByIdResponse>(
                    cancellationToken: cancellationToken);

                if (body?.Result?.Paper is { } p)
                    result.Add(p.MapToPaperDto());
            }
            catch
            {
                // skip unreachable / deleted papers
            }
        }

        return result;
    }

    public async Task<List<PaperBankInfoDto>> GetPaperBanksByIdsAsync(
        IEnumerable<Guid> paperIds,
        CancellationToken cancellationToken = default)
    {
        var idSet = paperIds.ToHashSet();
        if (idSet.Count == 0) return new List<PaperBankInfoDto>();

        var result = new List<PaperBankInfoDto>();

        foreach (var paperId in idSet)
        {
            try
            {
                var response = await labServiceApi.GetPaperBankByIdAsync(paperId);
                if (!response.IsSuccessStatusCode) continue;

                var body = await response.Content.ReadFromJsonAsync<LabGetPaperBankByIdResponse>(
                    cancellationToken: cancellationToken);

                if (body?.Result?.PaperBank is { } p)
                    result.Add(p.MapToDto());
            }
            catch
            {
                // skip unreachable / deleted papers
            }
        }

        return result;
    }

    public async Task<List<Guid>> GetExistingPaperBankIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var validIds = new List<Guid>();

        foreach (var id in ids)
        {
            try
            {
                var response = await labServiceApi.GetPaperBankByIdAsync(id);
                if (response.IsSuccessStatusCode)
                    validIds.Add(id);
            }
            catch
            {
                // If Lab service is unreachable or returns an error, skip this id
            }
        }

        return validIds;
    }

    public async Task<(List<PaperBankInfoDto> Items, long TotalCount)> GetPaperBanksByIdsPagedAsync(
        IEnumerable<Guid> paperIds,
        string? title = null,
        string? pages = null,
        string? number = null,
        string? volume = null,
        string? referenceContent = null,
        string[]? keywords = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var idSet = paperIds.ToHashSet();
        if (idSet.Count == 0)
            return (new List<PaperBankInfoDto>(), 0);

        // Fetch all paper-banks for the given IDs
        var allPapers = await GetPaperBanksByIdsAsync(idSet, cancellationToken);

        // Apply optional title filter (client-side, case-insensitive Contains)
        if (!string.IsNullOrWhiteSpace(title))
        {
            var search = title.Trim();
            allPapers = allPapers
                .Where(p => p.Title != null && p.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(pages))
        {
            var search = pages.Trim();
            allPapers = allPapers
                .Where(p => p.Pages != null && p.Pages.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(number))
        {
            var search = number.Trim();
            allPapers = allPapers
                .Where(p => p.Number != null && p.Number.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(volume))
        {
            var search = volume.Trim();
            allPapers = allPapers
                .Where(p => p.Volume != null && p.Volume.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(referenceContent))
        {
            var search = referenceContent.Trim();
            allPapers = allPapers
                .Where(p => p.ReferenceContent != null && p.ReferenceContent.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Apply optional keywords filter — AND semantics: paper must contain ALL requested keywords
        if (keywords is { Length: > 0 })
        {
            var normalizedKeywords = keywords
                .Select(k => k.Trim().ToLowerInvariant())
                .Where(k => k.Length > 0)
                .ToList();

            foreach (var keyword in normalizedKeywords)
            {
                var local = keyword;
                allPapers = allPapers
                    .Where(p => p.Keywords.Any(k => k.ToLowerInvariant().Contains(local)))
                    .ToList();
            }
        }

        var totalCount = allPapers.Count;

        var skip = (pageNumber - 1) * pageSize;

        var items = allPapers
            .Skip(skip)
            .Take(pageSize)
            .ToList();

        return (items, totalCount);
    }

    public async Task<(List<PaperInfoDto> Items, long TotalCount)> GetPapersByIdsPagedAsync(
        IEnumerable<Guid> paperIds,
        string? title = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var idSet = paperIds.ToHashSet();
        if (idSet.Count == 0)
            return (new List<PaperInfoDto>(), 0);

        // Fetch all papers for the given IDs
        var allPapers = await GetPapersByIdsAsync(idSet, cancellationToken);

        // Apply optional title filter (client-side)
        if (!string.IsNullOrWhiteSpace(title))
        {
            var search = title.Trim();
            allPapers = allPapers
                .Where(p => p.Title != null && p.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var totalCount = allPapers.Count;

        // Apply paging
        var skip = (pageNumber - 1) * pageSize;
        var items = allPapers
            .Skip(skip)
            .Take(pageSize)
            .ToList();

        return (items, totalCount);
    }

    public async Task<bool> DeletePaperBankAsync(
        Guid paperId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await labServiceApi.DeletePaperBankAsync(paperId);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            // If Lab service is unreachable or returns an error, return false
            return false;
        }
    }
    public async Task<bool> DeletePaperAsync(
        Guid paperId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await labServiceApi.DeletePaperAsync(paperId);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            // If Lab service is unreachable or returns an error, return false
            return false;
        }
    }
    public async Task<bool> CreatePaperContributorAsync(
        string sectionRole,
        Guid paperId,
        List<Guid> memberIds,
        Guid markSectionId,
        Guid? sectionId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await labServiceApi.CreatePaperContributorAsync(new CreatePaperContributorRequest
            {
                SectionRole   = sectionRole,
                PaperId       = paperId,
                MemberIds     = memberIds,
                MarkSectionId = markSectionId,
                SectionId     = sectionId
            });
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<LabPaperContributorDto>> GetPaperAuthorsAsync(
        Guid paperId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await labServiceApi.GetPaperAuthorsAsync(paperId);
            if (!response.IsSuccessStatusCode)
                return new List<LabPaperContributorDto>();

            var body = await response.Content
                .ReadFromJsonAsync<LabGetPaperAuthorsResponse>(cancellationToken: cancellationToken);

            return body?.Result?.Items?
                .Select(x => new LabPaperContributorDto
                {
                    Id            = x.Id,
                    PaperId       = x.PaperId,
                    MemberId      = x.MemberId,
                    MarkSectionId = x.AuthorRoleId,
                    SectionId     = null,
                    SectionRole   = x.AuthorRoleName,
                    UserId        = x.MemberId
                })
                .ToList() ?? new List<LabPaperContributorDto>();
        }
        catch
        {
            return new List<LabPaperContributorDto>();
        }
    }

    public async Task<List<LabPaperContributorDto>> GetPaperContributorsAsync(
        Guid paperId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await labServiceApi.GetPaperContributorsAsync(paperId);
            if (!response.IsSuccessStatusCode)
                return new List<LabPaperContributorDto>();

            var body = await response.Content.ReadFromJsonAsync<LabGetPaperContributorsResponse>(
                cancellationToken: cancellationToken);

            return body?.Result?.Items?
                .Select(x => new LabPaperContributorDto
                {
                    Id            = x.Id,
                    PaperId       = x.PaperId,
                    MemberId      = x.MemberId,
                    MarkSectionId = x.MarkSectionId,
                    SectionId     = x.SectionId,
                    SectionRole   = x.SectionRole,
                    UserId        = x.UserId
                })
                .ToList() ?? new List<LabPaperContributorDto>();
        }
        catch
        {
            return new List<LabPaperContributorDto>();
        }
    }

    public async Task<bool> UpdateProjectRulesAsync(
        IEnumerable<Guid> paperIds,
        string? context,
        string? domain,
        string? keypoint,
        CancellationToken cancellationToken = default)
    {
        var ids = paperIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (ids.Count == 0)
            return true;

        try
        {
            var response = await labServiceApi.UpdateProjectRulesAsync(new UpdateProjectRulesRequest
            {
                PaperIds = ids,
                Context = context,
                Domain = domain,
                Keypoint = keypoint
            });

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(List<PaperInfoDto> Items, long TotalCount)> GetAssignedPapersAsync(
        string? title = null,
        string[]? author = null,
        string? publisher = null,
        string? @abstract = null,
        string? doi = null,
        int? status = null,
        DateTimeOffset? fromPublicationDate = null,
        DateTimeOffset? toPublicationDate = null,
        string? paperType = null,
        string? journalName = null,
        string? conferenceName = null,
        string[]? tag = null,
        int pageNumber = 1,
        int pageSize = 1000,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await labServiceApi.GetAssignedPapersAsync(
                pageNumber,
                pageSize,
                title,
                author,
                publisher,
                @abstract,
                doi,
                status,
                fromPublicationDate,
                toPublicationDate,
                paperType,
                journalName,
                conferenceName,
                tag);

            if (!response.IsSuccessStatusCode)
                return ([], 0);

            var body = await response.Content.ReadFromJsonAsync<Common.Models.Reponses.ApiGetResponse<AssignedPapersPagedResult>>(
                cancellationToken: cancellationToken);

            return (body?.Result?.Items ?? [], body?.Result?.Paging?.TotalCount ?? 0);
        }
        catch
        {
            return ([], 0);
        }
    }

    public async Task<SubmissionStatusSummaryResult> GetSubmissionStatusSummaryAsync(
        IEnumerable<Guid> paperIds,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var ids = paperIds.ToList();
            if (ids.Count == 0)
                return new SubmissionStatusSummaryResult();

            var response = await labServiceApi.GetSubmissionStatusSummaryAsync(
                new LabSubmissionStatusSummaryRequest { PaperIds = ids });

            if (!response.IsSuccessStatusCode)
                return new SubmissionStatusSummaryResult();

            var body = await response.Content
                .ReadFromJsonAsync<LabSubmissionStatusSummaryResponse>(
                    cancellationToken: cancellationToken);

            return new SubmissionStatusSummaryResult
            {
                Items = body?.Items?
                    .Select(x => new SubmissionStatusSummaryItem
                    {
                        Status = x.Status,
                        Count = x.Count,
                    })
                    .ToList() ?? [],
            };
        }
        catch
        {
            return new SubmissionStatusSummaryResult();
        }
    }

    public async Task<bool> DeletePaperContributorAsync(
        Guid contributorId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await labServiceApi.DeletePaperContributorAsync(contributorId);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<LabSectionDto>> GetSectionsByPaperIdAsync(
        Guid paperId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await labServiceApi.GetSectionsByPaperIdAsync(paperId);
            if (!response.IsSuccessStatusCode)
                return new List<LabSectionDto>();

            var body = await response.Content
                .ReadFromJsonAsync<LabGetSectionsResponse>(cancellationToken: cancellationToken);

            return body?.Result?.Items?
                .Select(s => new LabSectionDto
                {
                    Id              = s.Id,
                    Title           = s.Title,
                    DisplayOrder    = s.DisplayOrder,
                    ParentSectionId = s.ParentSectionId,
                    PaperId         = s.PaperId
                })
                .ToList() ?? new List<LabSectionDto>();
        }
        catch
        {
            return new List<LabSectionDto>();
        }
    }

    public async Task<LabAdminDashboardKpisDto> GetAdminDashboardKpisAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await labServiceApi.GetAdminDashboardKpisAsync();
            if (!response.IsSuccessStatusCode)
                return new LabAdminDashboardKpisDto();

            var body = await response.Content.ReadFromJsonAsync<LabAdminDashboardKpisResponse>(
                cancellationToken: cancellationToken);

            if (body is null)
                return new LabAdminDashboardKpisDto();

            return new LabAdminDashboardKpisDto
            {
                PaperBankTotal = body.PaperBankTotal,
                JournalTotal = body.JournalTotal,
                ConferenceTotal = body.ConferenceTotal,
                TemplateTotal = body.TemplateTotal,
                SubmissionStatusCounts = body.SubmissionStatusCounts
                    .Select(x => new LabSubmissionStatusCountDto { Status = x.Status, Count = x.Count })
                    .ToList(),
                RecentPapers = body.RecentPapers
                    .Select(x => new LabRecentPaperDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Status = x.Status,
                        ConferenceJournalName = x.ConferenceJournalName,
                        CreatedAt = x.CreatedAt
                    })
                    .ToList()
            };
        }
        catch
        {
            return new LabAdminDashboardKpisDto();
        }
    }

    #endregion
}

// POST /papers/submission-status-summary response shape
[ExcludeFromCodeCoverage]
file sealed class LabSubmissionStatusSummaryItemRaw
{
    public int Status { get; set; }
    public int Count { get; set; }
}

[ExcludeFromCodeCoverage]
file sealed class LabSubmissionStatusSummaryResponse
{
    public List<LabSubmissionStatusSummaryItemRaw> Items { get; set; } = [];
}

[ExcludeFromCodeCoverage]
file static class LabPaperItemMapper
{
    internal static PaperBankInfoDto MapToDto(this LabPaperItem p) => new()
    {
        Id                   = p.Id,
        Title                = p.Title,
        Authors              = p.Authors,
        Publisher            = p.Publisher,
        Ranking              = p.Ranking,
        Abstract             = p.Abstract,
        Doi                  = p.Doi,
        Url                  = p.Url,
        FilePath             = p.FilePath,
        BibFilePath          = p.BibFilePath,
        ParsedText           = p.ParsedText,
        IsIngested           = p.IsIngested,
        IsAutoTagged         = p.IsAutoTagged,
        PublicationDate      = p.PublicationDate,
        Pages                = p.Pages,
        Number               = p.Number,
        Volume               = p.Volume,
        ConferenceJournalId  = p.ConferenceJournalId,
        ConferenceJournalName = p.ConferenceJournalName,
        ConferenceJournalType = p.ConferenceJournalType,
        ReferenceContent     = p.ReferenceContent,
        Keywords             = p.Keywords,
        IngestStatus         = p.IngestStatus,
        CreatedBy            = p.CreatedBy
    };

    internal static PaperInfoDto MapToPaperDto(this LabPaperFull p) => new()
    {
        Id                      = p.Id,
        Title                   = p.Title,
        Template                = p.Template,
        FilePath                = p.FilePath,
        Context                 = p.Context,
        Abstract                = p.Abstract,
        ResearchGap             = p.ResearchGap,
        MainContribution        = p.MainContribution,
        ResearchAim             = p.ResearchAim,
        Rule                    = p.Rule,
        ConferenceJournalId     = p.ConferenceJournalId,
        ConferenceJournalName   = p.ConferenceJournalName,
        ConferenceJournalType   = p.ConferenceJournalType,
        ConferenceJournalStartAt = p.ConferenceJournalStartAt,
        ConferenceJournalEndAt  = p.ConferenceJournalEndAt,
        SubProjectId            = p.SubProjectId,
        Status                  = p.Status,
        SubmissionStatus        = p.SubmissionStatus,
        CreatedBy               = p.CreatedBy
    };
}

// GET /admin/dashboard/kpis response shape
[ExcludeFromCodeCoverage]
file sealed class LabAdminDashboardKpisResponse
{
    public long PaperBankTotal { get; set; }
    public long JournalTotal { get; set; }
    public long ConferenceTotal { get; set; }
    public long TemplateTotal { get; set; }
    public List<LabAdminSubmissionStatusCountRaw> SubmissionStatusCounts { get; set; } = [];
    public List<LabAdminRecentPaperRaw> RecentPapers { get; set; } = [];
}

[ExcludeFromCodeCoverage]
file sealed class LabAdminSubmissionStatusCountRaw
{
    public int Status { get; set; }
    public int Count { get; set; }
}

[ExcludeFromCodeCoverage]
file sealed class LabAdminRecentPaperRaw
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public int? Status { get; set; }
    public string? ConferenceJournalName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
