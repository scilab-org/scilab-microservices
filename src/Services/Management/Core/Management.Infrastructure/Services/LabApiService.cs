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
    Released = 4
}

// Internal shapes matching Lab service response
file sealed class LabPaperItem
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public string? FilePath { get; set; }
    public int? Status { get; set; }
    public DateTimeOffset? PublicationDate { get; set; }
    public string? PaperType { get; set; }
    public string? JournalName { get; set; }
    public string? ConferenceName { get; set; }
}

// GET /papers  =>  { "result": { "items": [...], "paging": {...} } }
file sealed class LabGetPapersResult
{
    public List<LabPaperItem> Items { get; set; } = new();
}

file sealed class LabGetPapersResponse
{
    public LabGetPapersResult? Result { get; set; }
}

// GET /papers/{id}  =>  { "result": { "paper": { ... } } }
file sealed class LabGetPaperByIdResult
{
    public LabPaperItem? Paper { get; set; }
}

file sealed class LabGetPaperByIdResponse
{
    public LabGetPaperByIdResult? Result { get; set; }
}

public sealed class LabApiService(ILabServiceApi labServiceApi) : ILabApiService
{
    #region Implementations

    public async Task<List<PaperInfoDto>> GetAvailablePapersAsync(
        IEnumerable<Guid> existingPaperIds,
        string? searchText = null,
        CancellationToken cancellationToken = default)
    {
        var existingSet = existingPaperIds.ToHashSet();

        var response = await labServiceApi.GetPapersAsync(
            pageNumber: 1,
            pageSize: 1000,
            title: searchText);

        if (!response.IsSuccessStatusCode)
            return new List<PaperInfoDto>();

        var body = await response.Content.ReadFromJsonAsync<LabGetPapersResponse>(
            cancellationToken: cancellationToken);

        var allPapers = body?.Result?.Items ?? new List<LabPaperItem>();

        return allPapers
            .Where(p => !existingSet.Contains(p.Id))
            .Select(p => new PaperInfoDto
            {
                Id = p.Id,
                Title = p.Title,
                Abstract = p.Abstract,
                Doi = p.Doi,
                FilePath = p.FilePath,
                Status = p.Status.HasValue ? Enum.GetName(typeof(PaperStatus), p.Status.Value) : null,
                PublicationDate = p.PublicationDate,
                PaperType = p.PaperType,
                JournalName = p.JournalName,
                ConferenceName = p.ConferenceName
            })
            .ToList();
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
                    result.Add(new PaperInfoDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Abstract = p.Abstract,
                        Doi = p.Doi,
                        FilePath = p.FilePath,
                        Status = p.Status.HasValue ? Enum.GetName(typeof(PaperStatus), p.Status.Value) : null,
                        PublicationDate = p.PublicationDate,
                        PaperType = p.PaperType,
                        JournalName = p.JournalName,
                        ConferenceName = p.ConferenceName
                    });
            }
            catch
            {
                // skip unreachable / deleted papers
            }
        }

        return result;
    }

    public async Task<List<Guid>> GetExistingPaperIdsAsync(
        IEnumerable<Guid> paperIds,
        CancellationToken cancellationToken = default)
    {
        var validIds = new List<Guid>();

        foreach (var paperId in paperIds)
        {
            try
            {
                var response = await labServiceApi.GetPaperByIdAsync(paperId);
                if (response.IsSuccessStatusCode)
                    validIds.Add(paperId);
            }
            catch
            {
                // If user service is unreachable or returns an error, skip this userId
            }
        }

        return validIds;
    }
    #endregion
}
