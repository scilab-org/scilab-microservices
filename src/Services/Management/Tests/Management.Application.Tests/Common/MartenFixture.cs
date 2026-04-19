using Marten;
using Management.Domain.Entities;
using Testcontainers.PostgreSql;
using Weasel.Core;

namespace Management.Application.Tests.Common;

public class MartenFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:17.7")
        .Build();

    public IDocumentStore Store { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        Store = DocumentStore.For(opts =>
        {
            opts.Connection(_postgres.GetConnectionString());
            opts.AutoCreateSchemaObjects = AutoCreate.All;

            opts.Schema.For<ProjectEntity>();
            opts.Schema.For<MemberEntity>();
            opts.Schema.For<DatasetEntity>();
        });
    }

    public async Task DisposeAsync()
    {
        Store?.Dispose();
        await _postgres.DisposeAsync();
    }

    public IDocumentSession CreateSession() => Store.LightweightSession();

    public async Task CleanAsync()
    {
        await using var session = Store.LightweightSession();
        session.DeleteWhere<ProjectEntity>(_ => true);
        session.DeleteWhere<MemberEntity>(_ => true);
        session.DeleteWhere<DatasetEntity>(_ => true);
        await session.SaveChangesAsync();
    }
}
