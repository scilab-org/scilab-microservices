using Lab.Application.Dtos.Papers;
using Lab.Application.Dtos.Tags;

namespace Lab.Application.Models.Results;

public class GetTagByIdResult
{
    #region Fields, Properties and Indexers

    public TagDto Tag { get; init; }

    #endregion

    #region Ctors
    public GetTagByIdResult(TagDto tag)
    {
        Tag = tag;
    }

    #endregion
}