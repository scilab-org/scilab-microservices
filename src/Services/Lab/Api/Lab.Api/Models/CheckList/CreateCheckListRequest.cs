namespace Lab.Api.Models.CheckList;

public class CreateCheckListRequest
{
    public string Section { get; set; } = null!;
    public string RuleName { get; set; } = null!;
    public string Item { get; set; } = null!;
    public int Weight { get; set; }
}
