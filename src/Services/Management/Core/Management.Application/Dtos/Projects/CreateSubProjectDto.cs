namespace Management.Application.Dtos.Projects;

public class CreateSubProjectDto
{
    public Guid PaperId { get; set; }
    public string? Name { get; set; }
}