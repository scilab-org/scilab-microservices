using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcing.Events.Lab;

public sealed record PaperIngestionCompletedEvent : IntegrationEvent
{
    public Guid PaperId { get; init; }
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
}
