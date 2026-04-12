namespace User.Api.Tests.Endpoints;

public sealed class SystemEndpointTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;

    public SystemEndpointTests(ApiTestFactory factory)
    {
        _factory = factory;
        factory.SenderMock.Reset();
    }

    // ==========================================
    // POST /admin/system/initialize-data — InitializeData
    // NOTE: Endpoint is currently stubbed — it returns the current UserContext
    //       directly without dispatching a command. The "admin" policy is
    //       satisfied by any authenticated user in the test environment.
    // ==========================================

    [Fact]
    public async Task InitializeData_WhenAuthenticated_Returns200()
    {
        var client = _factory.CreateTestClient("app:user");

        var response = await client.PostAsync("/admin/system/initialize-data", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
