using System;
using System.Collections.Generic;
using System.Text;

namespace Lab.Worker.Outbox.Structs;

public record struct OutboxUpdate(
    Guid Id,
    DateTimeOffset ProcessedOnUtc,
    string? LastErrorMessage,
    int AttemptCount,
    DateTimeOffset? NextAttemptOnUtc);
