using AutoMapper;
using Management.Application.Features.Dataset.Commands;
using Management.Application.Features.Dataset.Queries;
using Management.Application.Mappings;
using Management.Application.Tests.Common;
using MediatR;

namespace Management.Application.Tests.Integration;

#region DeleteDatasetCommand

[Collection("MartenIntegration")]
public class DeleteDatasetCommandHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public DeleteDatasetCommandHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowClientValidation_WhenDatasetDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var handler = new DeleteDatasetCommandHandler(session);
        var command = new DeleteDatasetCommand(Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task Handle_Should_DeleteDataset_AndRemoveFromProjects()
    {
        await using var session = _fixture.CreateSession();
        var datasetId = Guid.NewGuid();
        var dataset = DatasetEntity.Create(datasetId, "Test Dataset", "Desc");
        session.Store(dataset);

        var projectId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        project.DatasetIds.Add(datasetId);
        session.Store(project);
        await session.SaveChangesAsync();

        var handler = new DeleteDatasetCommandHandler(session);
        var command = new DeleteDatasetCommand(datasetId);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
        var deletedDataset = await session.LoadAsync<DatasetEntity>(datasetId);
        deletedDataset.Should().BeNull();
        var updatedProject = await session.LoadAsync<ProjectEntity>(projectId);
        updatedProject!.DatasetIds.Should().NotContain(datasetId);
    }

    [Fact]
    public async Task Handle_Should_DeleteDataset_WhenNotInAnyProject()
    {
        await using var session = _fixture.CreateSession();
        var datasetId = Guid.NewGuid();
        var dataset = DatasetEntity.Create(datasetId, "Orphan Dataset", null);
        session.Store(dataset);
        await session.SaveChangesAsync();

        var handler = new DeleteDatasetCommandHandler(session);
        var command = new DeleteDatasetCommand(datasetId);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
    }
}

#endregion

#region GetDatasetsQuery

[Collection("MartenIntegration")]
public class GetDatasetsQueryHandlerIntegrationTests : IAsyncLifetime
{
    private readonly MartenFixture _fixture;
    public GetDatasetsQueryHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    public async Task InitializeAsync() => await _fixture.CleanAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    private static IMapper CreateMapper() =>
        new MapperConfiguration(cfg => cfg.AddProfile<ManagementMappingProfile>()).CreateMapper();

    [Fact]
    public async Task Handle_Should_ReturnEmpty_WhenProjectNotFound_WithProjectFilter()
    {
        await using var session = _fixture.CreateSession();
        var handler = new GetDatasetsQueryHandler(session, CreateMapper());
        var query = new GetDatasetsQuery(Guid.NewGuid(), new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnAllDatasets_WhenNoProjectFilter()
    {
        await using var session = _fixture.CreateSession();
        session.Store(DatasetEntity.Create(Guid.NewGuid(), "Dataset1", null));
        session.Store(DatasetEntity.Create(Guid.NewGuid(), "Dataset2", null));
        await session.SaveChangesAsync();

        var handler = new GetDatasetsQueryHandler(session, CreateMapper());
        var query = new GetDatasetsQuery(null, new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_Should_ReturnFilteredDatasets_WhenProjectFilter()
    {
        await using var session = _fixture.CreateSession();
        var d1 = Guid.NewGuid();
        var d2 = Guid.NewGuid();
        session.Store(DatasetEntity.Create(d1, "InProject", null));
        session.Store(DatasetEntity.Create(d2, "NotInProject", null));

        var projectId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        project.DatasetIds.Add(d1);
        session.Store(project);
        await session.SaveChangesAsync();

        var handler = new GetDatasetsQueryHandler(session, CreateMapper());
        var query = new GetDatasetsQuery(projectId, new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("InProject");
    }

    [Fact]
    public async Task Handle_Should_ReturnPaginatedResults()
    {
        await using var session = _fixture.CreateSession();
        for (int i = 0; i < 5; i++)
            session.Store(DatasetEntity.Create(Guid.NewGuid(), $"D{i}", null));
        await session.SaveChangesAsync();

        var handler = new GetDatasetsQueryHandler(session, CreateMapper());
        var query = new GetDatasetsQuery(null, new PaginationRequest(1, 2));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Paging.TotalCount.Should().Be(5);
    }
}

#endregion
