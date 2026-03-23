using System.Net.Http.Json;
using Management.Application.Dtos.Papers;
using Management.Application.Services;
using Management.Infrastructure.ApiClients;

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
// Internal deserialization shapes — LabPaperItem (bank/list) and LabPaperFull (single paper with Template + ParsedText)
file sealed class LabPaperItem
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public string? FilePath { get; set; }
    public int? Status { get; set; }
    public bool? IsIngested { get; set; }
    public bool? IsAutoTagged { get; set; }
    public DateTimeOffset? PublicationDate { get; set; }
    public string? PaperType { get; set; }
    public string? JournalName { get; set; }
    public string? ConferenceName { get; set; }
    public List<string> TagNames { get; set; } = new();
}

// GET /papers/sample  =>  { "result": { "items": [...], "paging": {...} } }
file sealed class LabGetPapersResult
{
    public List<LabPaperItem> Items { get; set; } = new();
}

file sealed class LabGetPapersResponse
{
    public LabGetPapersResult? Result { get; set; }
}

// GET /paper-bank  =>  { "result": { "items": [...], "paging": { "totalCount": ... } } }
file sealed class LabGetPaperBanksPaging
{
    public long TotalCount { get; set; }
}

file sealed class LabGetPaperBanksResult
{
    public List<LabPaperItem> Items { get; set; } = new();
    public LabGetPaperBanksPaging? Paging { get; set; }
}

file sealed class LabGetPaperBanksResponse
{
    public LabGetPaperBanksResult? Result { get; set; }
}

// GET /papers/{id}  =>  { "result": { "paper": { ...PaperDto... } } }
// PaperDto adds Template + ParsedText over PaperBankInfoDto
file sealed class LabPaperFull
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Template { get; set; }
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public string? FilePath { get; set; }
    public int? Status { get; set; }
    public string? ParsedText { get; set; }
    public bool? IsIngested { get; set; }
    public bool? IsAutoTagged { get; set; }
    public DateTimeOffset? PublicationDate { get; set; }
    public string? PaperType { get; set; }
    public string? JournalName { get; set; }
    public string? ConferenceName { get; set; }
    public List<string> TagNames { get; set; } = new();
    public string? CreatedBy { get; set; }
}

file sealed class LabGetPaperByIdResult
{
    public LabPaperFull? Paper { get; set; }
}

file sealed class LabGetPaperByIdResponse
{
    public LabGetPaperByIdResult? Result { get; set; }
}

// GET /paper-bank/{id}  =>  { "result": { "paperBank": { ... } } }
file sealed class LabGetPaperBankByIdResult
{
    public LabPaperItem? PaperBank { get; set; }
}

file sealed class LabGetPaperBankByIdResponse
{
    public LabGetPaperBankByIdResult? Result { get; set; }
}

// GET /papers/{id}/sections  =>  { "result": { "items": [...] } }
file sealed class LabSectionItem
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public float DisplayOrder { get; set; }
    public Guid? ParentSectionId { get; set; }
    public Guid PaperId { get; set; }
    public string? SectionRole { get; set; }
}

file sealed class LabGetSectionsResult
{
    public List<LabSectionItem>? Items { get; set; }
}

file sealed class LabGetSectionsResponse
{
    public LabGetSectionsResult? Result { get; set; }
}

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

file sealed class LabGetPaperContributorsResult
{
    public List<LabPaperContributorItem> Items { get; set; } = new();
}

file sealed class LabGetPaperContributorsResponse
{
    public LabGetPaperContributorsResult? Result { get; set; }
}

public sealed class LabApiService(ILabServiceApi labServiceApi) : ILabApiService
{
    #region Implementations

    public async Task<(List<PaperBankInfoDto> Items, long TotalCount)> GetAvailablePapersAsync(
        IEnumerable<Guid> existingPaperIds,
        string? title = null,
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
        var existingSet = existingPaperIds.ToHashSet();

        var response = await labServiceApi.GetPaperBanksAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            title: title,
            @abstract: @abstract,
            doi: doi,
            status: status,
            fromPublicationDate: fromPublicationDate,
            toPublicationDate: toPublicationDate,
            paperType: paperType,
            journalName: journalName,
            conferenceName: conferenceName,
            tag: tag);

        if (!response.IsSuccessStatusCode)
            return (new List<PaperBankInfoDto>(), 0);

        var body = await response.Content.ReadFromJsonAsync<LabGetPaperBanksResponse>(
            cancellationToken: cancellationToken);

        var allItems = body?.Result?.Items ?? new List<LabPaperItem>();

        var filtered = allItems
            .Where(p => !existingSet.Contains(p.Id))
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
        string[]? tags = null,
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

        // Apply optional tags filter — AND semantics: paper must contain ALL requested tags
        if (tags is { Length: > 0 })
        {
            var normalizedTags = tags
                .Select(t => t.Trim().ToLowerInvariant())
                .Where(t => t.Length > 0)
                .ToList();

            foreach (var tag in normalizedTags)
            {
                var local = tag;
                allPapers = allPapers
                    .Where(p => p.TagNames.Any(t => t.ToLowerInvariant().Contains(local)))
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
        Guid memberId,
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
                MemberId      = memberId,
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

    #endregion
}

file static class LabPaperItemMapper
{
    internal static PaperBankInfoDto MapToDto(this LabPaperItem p) => new()
    {
        Id              = p.Id,
        Title           = p.Title,
        Abstract        = p.Abstract,
        Doi             = p.Doi,
        FilePath        = p.FilePath,
        Status          = p.Status ?? 0,
        PublicationDate = p.PublicationDate,
        PaperType       = p.PaperType,
        JournalName     = p.JournalName,
        ConferenceName  = p.ConferenceName,
        TagNames        = p.TagNames
    };

    internal static PaperInfoDto MapToPaperDto(this LabPaperFull p) => new()
    {
        Id              = p.Id,
        Title           = p.Title,
        Template        = p.Template,
        Abstract        = p.Abstract,
        Doi             = p.Doi,
        FilePath        = p.FilePath,
        Status          = p.Status ?? 0,
        ParsedText      = p.ParsedText,
        PublicationDate = p.PublicationDate,
        PaperType       = p.PaperType,
        JournalName     = p.JournalName,
        ConferenceName  = p.ConferenceName,
        TagNames        = p.TagNames,
        CreatedBy       = p.CreatedBy
    };
}
