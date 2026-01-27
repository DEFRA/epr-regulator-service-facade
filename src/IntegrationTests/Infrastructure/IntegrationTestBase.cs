namespace IntegrationTests.Infrastructure;

using WireMock.Server;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    private FacadeWebApplicationFactory Factory { get; set; } = null!;
    protected HttpClient Client { get; private set; } = null!;
    protected WireMockServer CommonDataApiServer => Factory.CommonDataApiServer;
    protected WireMockServer SubmissionsApiServer => Factory.SubmissionsApiServer;

    public virtual Task InitializeAsync()
    {
        Factory = new FacadeWebApplicationFactory();
        Client = Factory.CreateClient();
        CommonDataApiServer.ResetMappings(); // Avoid mystery-guest of json based default mappings. Tests should define their own required data explicitly
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        await Factory.DisposeAsync();
    }
}
