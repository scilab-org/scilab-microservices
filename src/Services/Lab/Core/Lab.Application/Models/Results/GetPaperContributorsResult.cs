using Lab.Application.Dtos.PaperContributors;

namespace Lab.Application.Models.Results;

public class GetPaperContributorsResult
{
    public Guid PaperId { get; init; }
    public List<PaperContributorDto> Items { get; init; }

    public GetPaperContributorsResult(Guid paperId, List<PaperContributorDto> items)
    {
        PaperId = paperId;
        Items = items;
    }
}

