namespace IntegrationTests.Infrastructure;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using WireMock.Server;

public class FacadeWebApplicationFactory : WebApplicationFactory<EPR.RegulatorService.Facade.API.Program>
{
    private readonly WireMockServer _commonDataApiServer = MockCommonDataApi.MockCommonDataApiServer.Start(useSsl: true);
    private readonly WireMockServer _submissionsApiServer;

    public WireMockServer CommonDataApiServer => _commonDataApiServer;
    public WireMockServer SubmissionsApiServer => _submissionsApiServer;

    public FacadeWebApplicationFactory()
    {
        // Start a separate WireMock for the Submissions API
        _submissionsApiServer = WireMock.Server.WireMockServer.Start();
        SetupDefaultSubmissionsApiMock();
    }

    private void SetupDefaultSubmissionsApiMock()
    {
        // Mock the submission events endpoint that's called for delta updates
        _submissionsApiServer.Given(WireMock.RequestBuilders.Request.Create()
                .UsingGet()
                .WithPath(new WireMock.Matchers.WildcardMatcher("/v*/submissions/events/organisation-registration*", true)))
            .RespondWith(WireMock.ResponseBuilders.Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("[]"));
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set environment variables for the API endpoints
        Environment.SetEnvironmentVariable("CommonDataApiConfig__BaseUrl", $"{_commonDataApiServer.Url}/api/");
        Environment.SetEnvironmentVariable("SubmissionsApiConfig__BaseUrl", $"{_submissionsApiServer.Url}");

        builder.ConfigureTestServices(services =>
        {
            // Remove the existing authentication services
            var authDescriptors = services.Where(d => d.ServiceType.FullName?.Contains("Authentication") == true).ToList();
            foreach (var descriptor in authDescriptors)
            {
                services.Remove(descriptor);
            }

            // Replace with test authentication handler
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _commonDataApiServer?.Stop();
            _commonDataApiServer?.Dispose();
            _submissionsApiServer?.Stop();
            _submissionsApiServer?.Dispose();
        }
        base.Dispose(disposing);
    }
}
