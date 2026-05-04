namespace Lab.Application.Dtos.CheckLists;

public class CreateCheckListDto
{
    public string Section { get; set; } = null!;
    public List<CheckListItemDto> Items { get; set; } = [];
}
