using Common.Models;
using Management.Application.Dtos.Datasets;
using Management.Application.Features.Dataset.Commands;
using Management.Application.Features.Dataset.Queries;
using Management.Application.Models.Results;

namespace Management.Api.Tests.Endpoints;

public sealed class DatasetEndpointTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;

    public DatasetEndpointTests(ApiTestFactory factory)
    {
        _factory = factory;
        factory.SenderMock.Reset();
    }

    [Fact]
    public async Task CreateDataset_Returns201()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<CreateDatasetCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var client = _factory.CreateTestClient();
        var form = new MultipartFormDataContent();
        form.Add(new StringContent("Test Dataset"), "Name");
        form.Add(new StringContent(Guid.NewGuid().ToString()), "ProjectId");

        var response = await client.PostAsync("/manager/datasets", form);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateDataset_WithFile_Returns201()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<CreateDatasetCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var client = _factory.CreateTestClient();
        var form = new MultipartFormDataContent();
        form.Add(new StringContent("Test Dataset"), "Name");
        form.Add(new StringContent(Guid.NewGuid().ToString()), "ProjectId");
        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        form.Add(fileContent, "File", "test.csv");

        var response = await client.PostAsync("/manager/datasets", form);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateDataset_WithEmptyProjectId_Returns400()
    {
        var client = _factory.CreateTestClient();
        var form = new MultipartFormDataContent();
        form.Add(new StringContent("Test Dataset"), "Name");
        form.Add(new StringContent(""), "ProjectId");

        var response = await client.PostAsync("/manager/datasets", form);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateDataset_WithInvalidProjectId_Returns400()
    {
        var client = _factory.CreateTestClient();
        var form = new MultipartFormDataContent();
        form.Add(new StringContent("Test Dataset"), "Name");
        form.Add(new StringContent("not-a-guid"), "ProjectId");

        var response = await client.PostAsync("/manager/datasets", form);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateDataset_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<UpdateDatasetCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var client = _factory.CreateTestClient();
        var form = new MultipartFormDataContent();
        form.Add(new StringContent("Updated"), "Name");

        var response = await client.PutAsync($"/manager/datasets/{Guid.NewGuid()}", form);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateDataset_WithFile_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<UpdateDatasetCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var client = _factory.CreateTestClient();
        var form = new MultipartFormDataContent();
        form.Add(new StringContent("Updated"), "Name");
        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        form.Add(fileContent, "File", "updated.csv");

        var response = await client.PutAsync($"/manager/datasets/{Guid.NewGuid()}", form);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteDataset_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<DeleteDatasetCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var client = _factory.CreateTestClient();
        var response = await client.DeleteAsync($"/manager/datasets/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDatasets_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetDatasetsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetDatasetsResult(new List<DatasetDto>(), 0, new PaginationRequest()));

        var client = _factory.CreateTestClient();
        var response = await client.GetAsync("/datasets");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDatasets_WithProjectFilter_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetDatasetsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetDatasetsResult(new List<DatasetDto>(), 0, new PaginationRequest()));

        var client = _factory.CreateTestClient();
        var response = await client.GetAsync($"/datasets?projectId={Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
