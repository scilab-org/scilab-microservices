using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Members;

[ExcludeFromCodeCoverage]
public sealed class DeleteProjectManagersDto
{
    public List<Guid> MemberIds { get; set; } = new();
}

