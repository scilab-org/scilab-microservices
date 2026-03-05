namespace Management.Application.Models.Filters;

public class GetProjectMembersFilter
{
    public string? SearchEmail { get; set; }
    public string? ProjectRole { get; set; }
}