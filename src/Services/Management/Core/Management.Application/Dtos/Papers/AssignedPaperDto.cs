namespace Management.Application.Dtos.Papers;

public class AssignedPaperDto : PaperInfoDto
{
    public Guid ProjectId { get; set; }
    public string? ProjectCode { get; set; }
}