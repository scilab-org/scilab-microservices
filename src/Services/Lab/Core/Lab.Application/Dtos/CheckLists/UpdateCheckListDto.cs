namespace Lab.Application.Dtos.CheckLists;

public class UpdateCheckListDto
{
    public string Section { get; set; } = null!;
    public List<CheckListItemDto> Items { get; set; } = [];
}
