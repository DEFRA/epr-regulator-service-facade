namespace MockCommonDataApi.CommonDataApi;

using System.Diagnostics.CodeAnalysis;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

[ExcludeFromCodeCoverage]
public static class CommonDataApi
{
    public static WireMockServer WithCommonDataApi(this WireMockServer server)
    {
        server.Given(Request.Create()
                .UsingGet()
                .WithPath("/api/submission-events/get-last-sync-time"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile("Responses/CommonDataApi/submission-events-last-sync-time.json"));

        server.Given(Request.Create()
                .UsingGet()
                .WithPath(new WireMock.Matchers.RegexMatcher("^/api/submissions/organisation-registrations/.*")))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile("Responses/CommonDataApi/organisation-registration-submissions.json"));

        return server;
    }
}
