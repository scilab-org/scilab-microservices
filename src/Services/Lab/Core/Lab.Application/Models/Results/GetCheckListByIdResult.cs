using Lab.Application.Dtos.CheckLists;

namespace Lab.Application.Models.Results;

public class GetCheckListByIdResult
{
    public CheckListDto CheckList { get; init; }

    public GetCheckListByIdResult(CheckListDto checkList)
    {
        CheckList = checkList;
    }
}
