using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Projects;

[ExcludeFromCodeCoverage]
public class CreateProjectConferenceJournalDto
{
    public List<Guid> ConferenceJournalIds { get; set; } = [];
}