using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Projects;

[ExcludeFromCodeCoverage]
public class DeleteProjectConferenceJournalDto
{
    public List<Guid> ConferenceJournalIds { get; set; } = [];
}