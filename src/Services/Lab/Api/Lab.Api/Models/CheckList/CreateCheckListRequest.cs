namespace Lab.Api.Models.CheckList;

public class CreateCheckListRequest
{
    public string Section { get; set; } = null!;
    public List<CheckListItemRequest> Items { get; set; } = [];
}
