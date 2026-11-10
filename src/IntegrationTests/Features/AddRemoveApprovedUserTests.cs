using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using IntegrationTests.Infrastructure;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace IntegrationTests.Features
{
    [Collection("Sequential")] // Shares the WireMock servers, so can't run in parallel
    public class AddRemoveApprovedUserTests : IntegrationTestBase
    {
        [Fact]
        public async Task WhenBackendReturnsConflict_ReturnsConflict_AndDoesNotRetry()
        {
            AccountsServer.Given(Request.Create()
                    .UsingPost()
                    .WithPath("/api/regulators/add-remove-approved-users"))
                .RespondWith(Response.Create()
                    .WithStatusCode(409)
                    .WithHeader("Content-Type", "application/problem+json")
                    .WithBody(JsonSerializer.Serialize(new
                    {
                        status = 409,
                        title = "USER_ALREADY_INVITED",
                        detail = "User with the supplied email is already invited"
                    })));

            var response = await Client.PostAsJsonAsync("/api/organisations/add-remove-approved-users", new
            {
                organisationId = Guid.NewGuid(),
                invitedPersonEmail = "invitee@test.example",
                invitedPersonFirstName = "Ina",
                invitedPersonLastName = "Vitee"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            AccountsServer.LogEntries.Should().ContainSingle(); // not retried
        }
    }
}
