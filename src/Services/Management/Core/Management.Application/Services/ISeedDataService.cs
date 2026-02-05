using Marten;

namespace Management.Application.Services;

public interface ISeedDataService
{
    #region Methods

    Task<bool> SeedDataAsync(IDocumentSession session, CancellationToken cancellationToken);

    #endregion
}