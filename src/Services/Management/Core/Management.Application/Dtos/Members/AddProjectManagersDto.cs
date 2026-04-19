using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Members;

[ExcludeFromCodeCoverage]
public class AddProjectManagersDto
{
    public Guid UserId { get; set; } = new();
}