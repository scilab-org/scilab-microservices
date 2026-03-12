using Lab.Application.Dtos.PaperContributors;

namespace Lab.Application.Models.Results;

public class GetPaperContributorsResult
{
    public List<PaperContributorDto> Items { get; init; }

    public GetPaperContributorsResult(List<PaperContributorDto> items)
    {
        Items = items;
    }
}

