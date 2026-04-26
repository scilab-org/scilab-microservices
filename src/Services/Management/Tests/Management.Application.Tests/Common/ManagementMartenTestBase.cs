using AutoMapper;
using Management.Application.Mappings;
using Management.Domain.Entities;
using Marten;
using Weasel.Core;

namespace Management.Application.Tests.Common;

public abstract class ManagementMartenTestBase : IAsyncLifetime
{
    private const string ConnectionString =
        "Host=127.0.0.1;Port=5432;Database=postgres;Username=postgres;Password=postgres";

    protected IDocumentStore Store { get; private set; } = null!;
    protected IDocumentSession Session { get; private set; } = null!;
    protected IMapper Mapper { get; private set; } = null!;

    protected abstract string SchemaName { get; }

    public async Task InitializeAsync()
    {
        Store = DocumentStore.For(opts =>
        {
            opts.Connection(ConnectionString);
            opts.AutoCreateSchemaObjects = AutoCreate.All;
            opts.DatabaseSchemaName = SchemaName;
            opts.Schema.For<AffiliationEntity>();
            opts.Schema.For<DomainEntity>();
            opts.Schema.For<UserAffiliationEntity>();
            opts.Schema.For<MemberEntity>();
        });

        await Store.Advanced.ResetAllData();
        Session = Store.LightweightSession();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<ManagementMappingProfile>());
        Mapper = config.CreateMapper();
    }

    public Task DisposeAsync()
    {
        Session.Dispose();
        Store.Dispose();
        return Task.CompletedTask;
    }
}
