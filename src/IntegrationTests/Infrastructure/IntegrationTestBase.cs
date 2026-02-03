namespace IntegrationTests.Infrastructure;

using System.Text.Json;
using FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    private FacadeWebApplicationFactory Factory { get; set; } = null!;
    protected HttpClient Client { get; private set; } = null!;
    protected WireMockServer CommonDataServer => Factory.CommonDataServer;

    public virtual Task InitializeAsync()
    {
        Factory = new FacadeWebApplicationFactory();
        Client = Factory.CreateClient();
        CommonDataServer.ResetMappings(); // Avoid mystery-guest of json based default mappings. Tests should define their own required data explicitly
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        await Factory.DisposeAsync();
    }

    protected void SetupCommonDataMockLastSyncTime(DateTime lastSyncTime)
    {
        CommonDataServer.Given(Request.Create()
                .UsingGet()
                .WithPath("/api/submission-events/get-last-sync-time"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(new
                {
                    lastSyncTime = lastSyncTime.ToString("yyyy-MM-ddTHH:mm:ssZ")
                })));
    }

    protected void SetupCommonDataMockOrganisationRegistrations(object[] data, int nationId = 1)
    {
        CommonDataServer.Given(Request.Create()
                .UsingGet()
                .WithPath($"/api/submissions/organisation-registrations/{nationId}"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(new
                {
                    items = data,
                    currentPage = 1,
                    pageSize = 20,
                    totalItems = data.Length
                })));
    }

    protected void SetupCommonDataMockOrganisationRegistrationDetails(Guid submissionId, object data)
    {
        CommonDataServer.Given(Request.Create()
                .UsingGet()
                .WithPath($"/api/submissions/organisation-registration-submission/{submissionId}"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(data)));
    }

    protected void SetupCommonDataMockPoMSubmissions(object[] data)
    {
        CommonDataServer.Given(Request.Create()
                .UsingPost()
                .WithPath("/api/submissions/pom/summary"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(new
                {
                    items = data,
                    currentPage = 1,
                    pageSize = 20,
                    totalItems = data.Length
                })));
    }

    protected void SetupCommonDataMockRegistrationSubmissions(object[] data)
    {
        CommonDataServer.Given(Request.Create()
                .UsingPost()
                .WithPath("/api/submissions/registrations/summary"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(new
                {
                    items = data,
                    currentPage = 1,
                    pageSize = 20,
                    totalItems = data.Length
                })));
    }

    protected void SetupCommonDataMockPomResubmissionPaycalParameters(Guid submissionId, object data)
    {
        CommonDataServer.Given(Request.Create()
                .UsingGet()
                .WithPath($"/api/submissions/pom-resubmission-paycal-parameters/{submissionId}"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(data)));
    }
}
