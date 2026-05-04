namespace Lab.Application.Dtos.CheckLists;

public class CreateCheckListDto
{
    public string Section { get; set; } = null!;
    public string RuleName { get; set; } = null!;
    public string Item { get; set; } = null!;
    public int Weight { get; set; }
}
