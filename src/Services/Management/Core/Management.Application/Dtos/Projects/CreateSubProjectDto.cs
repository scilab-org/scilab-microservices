using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Projects;

[ExcludeFromCodeCoverage]
public class CreateSubProjectDto
{
    public Guid PaperId { get; set; }
    public string? Name { get; set; }
}