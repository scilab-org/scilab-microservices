using Management.Application.Dtos.Members;
using System.Diagnostics.CodeAnalysis;
namespace Management.Application.Models.Results;

[ExcludeFromCodeCoverage]
public sealed class GetAvailableProjectUsersResult
{
    #region Fields, Properties and Indexers
    public List<UserInfoDto> Items { get; init; }
    public int TotalCount { get; init; }
    #endregion
    #region Ctors
    public GetAvailableProjectUsersResult(List<UserInfoDto> items)
    {
        Items = items;
        TotalCount = items.Count;
    }
    #endregion
}
