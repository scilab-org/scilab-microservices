using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Management.Application.Tests.Common;

public abstract class BaseTest
{
    protected static CancellationToken CancellationToken => CancellationToken.None;

    protected static ILogger<T> CreateLogger<T>() => NullLogger<T>.Instance;
}
