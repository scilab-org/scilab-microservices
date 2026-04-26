using AutoMapper;
using Lab.Application.Mappings;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Tests.Common;

public abstract class MartenTestBase : IAsyncLifetime
{
    private const string ConnectionString =
        "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres";

    protected IDocumentStore Store { get; private set; } = null!;
    protected IDocumentSession Session { get; private set; } = null!;
    protected IMapper Mapper { get; private set; } = null!;

    /// <summary>
    /// Override to provide a unique schema name per test class to avoid cross-test interference.
    /// </summary>
    protected abstract string SchemaName { get; }

    public async Task InitializeAsync()
    {
        Store = DocumentStore.For(opts =>
        {
            opts.Connection(ConnectionString);
            opts.AutoCreateSchemaObjects = Weasel.Core.AutoCreate.All;
            opts.DatabaseSchemaName = SchemaName;
            opts.Schema.For<KeywordEntity>().SoftDeleted();
            opts.Schema.For<PaperContributorEntity>().SoftDeleted();
        });

        await Store.Advanced.ResetAllData();

        Session = Store.LightweightSession();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<LabMappingProfile>());
        Mapper = config.CreateMapper();
    }

    public Task DisposeAsync()
    {
        Session.Dispose();
        Store.Dispose();
        return Task.CompletedTask;
    }
}
