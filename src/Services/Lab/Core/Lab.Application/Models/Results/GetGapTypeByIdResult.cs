using Lab.Application.Dtos.GapTypes;

namespace Lab.Application.Models.Results;

public class GetGapTypeByIdResult
{
    public GapTypeDto GapType { get; init; }

    public GetGapTypeByIdResult(GapTypeDto gapType)
    {
        GapType = gapType;
    }
}
