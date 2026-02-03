namespace MockAccountService.AccountsApi;

using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

public static class AccountsApi
{
    public static WireMockServer WithAccountsApi(this WireMockServer server)
    {
        // GET /api/users/user-organisations?userId={userId}
        server.Given(Request.Create()
                .UsingGet()
                .WithPath("/api/users/user-organisations")
                .WithParam("userId"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile("Responses/AccountsApi/user-organisations.json"));

        return server;
    }
}
