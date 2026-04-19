using Common.Models;
using Management.Application.Dtos.Papers;
using Management.Application.Dtos.Projects;
using Management.Application.Features.Project.Commands;
using Management.Application.Features.Project.Queries;
using Management.Application.Models.Results;

namespace Management.Api.Tests.Endpoints;

public sealed class PaperEndpointTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;

    public PaperEndpointTests(ApiTestFactory factory)
    {
        _factory = factory;
        factory.SenderMock.Reset();
    }

    [Fact]
    public async Task CreateProjectPaper_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<CreateProjectPaperCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { Guid.NewGuid() });

        var client = _factory.CreateTestClient();
        var response = await client.PostAsJsonAsync($"/manager/projects/{Guid.NewGuid()}/papers", new CreateProjectPaperDto { PaperIds = new List<Guid> { Guid.NewGuid() } });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAvailablePapers_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetAvailablePapersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAvailablePapersResult(new List<PaperBankInfoDto>(), 0, new PaginationRequest()));

        var client = _factory.CreateTestClient();
        var response = await client.GetAsync($"/manager/projects/{Guid.NewGuid()}/papers/available");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProjectPapers_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetProjectPapersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetProjectPapersResult(new List<PaperBankInfoDto>(), 0, new PaginationRequest()));

        var client = _factory.CreateTestClient();
        var response = await client.GetAsync($"/projects/{Guid.NewGuid()}/papers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteProjectPapers_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<DeleteProjectPapersCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { Guid.NewGuid() });

        var client = _factory.CreateTestClient();
        var response = await client.PostAsJsonAsync($"/manager/projects/{Guid.NewGuid()}/papers/remove", new DeleteProjectPaperDto { PaperIds = new List<Guid> { Guid.NewGuid() } });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteProjectPaperByBankId_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<DeleteProjectPaperByBankIdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { Guid.NewGuid() });

        var client = _factory.CreateTestClient();
        var response = await client.PostAsync($"/projects/paper-bank/{Guid.NewGuid()}", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProjectSubmissionStatusSummary_WhenAuthenticated_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetProjectSubmissionStatusSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SubmissionStatusSummaryResult());

        var client = _factory.CreateTestClient(Common.Constants.AuthorizeConstants.User);
        var response = await client.GetAsync($"/projects/{Guid.NewGuid()}/submission-status-summary");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateProjectConferenceJournal_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<CreateProjectConferenceJournalCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var client = _factory.CreateTestClient();
        var response = await client.PostAsync($"/manager/projects/{Guid.NewGuid()}/conference-journals/{Guid.NewGuid()}", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteProjectConferenceJournals_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<DeleteProjectConferenceJournalsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var client = _factory.CreateTestClient();
        var response = await client.PutAsync($"/manager/projects/{Guid.NewGuid()}/conference-journals/{Guid.NewGuid()}", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteProjectConferenceJournalByJournalId_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<DeleteProjectConferenceJournalByJournalIdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { Guid.NewGuid() });

        var client = _factory.CreateTestClient();
        var response = await client.PostAsync($"/projects/conference-journals/{Guid.NewGuid()}", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
