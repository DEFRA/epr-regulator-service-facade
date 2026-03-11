namespace MockSubmissionsApi.SubmissionsApi;

using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

public static class SubmissionsApi
{
    public static WireMockServer WithSubmissionsApi(this WireMockServer server)
    {
        server.Given(Request.Create()
                .UsingPost()
                .WithPath("/v1/submissions/*/events"))
            .RespondWith(Response.Create()
                .WithStatusCode(201)
                .WithHeader("Content-Type", "application/json"));

        server.Given(Request.Create()
                .UsingGet()
                .WithPath("/v1/submissions/events/get-regulator-pom-decision"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile("Responses/SubmissionsApi/get-regulator-pom-decision.json"));

        server.Given(Request.Create()
                .UsingGet()
                .WithPath("/v1/submissions/events/get-regulator-registration-decision"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile("Responses/SubmissionsApi/get-regulator-registration-decision.json"));

        server.Given(Request.Create()
                .UsingGet()
                .WithPath("/v1/submissions/events/organisation-registration"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile("Responses/SubmissionsApi/organisation-registration-events.json"));

        return server;
    }
}
