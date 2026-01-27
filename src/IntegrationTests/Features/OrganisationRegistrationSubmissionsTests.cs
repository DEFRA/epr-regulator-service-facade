namespace IntegrationTests.Features;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using Infrastructure;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public class OrganisationRegistrationSubmissionsTests : IntegrationTestBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public override Task InitializeAsync()
    {
        base.InitializeAsync();
        SetupLastSyncTimeMock();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetRegistrationSubmissionList_ReturnsAllRegistrationJourneyTypes()
    {
        // Arrange
        SetupCommonDataApiMock([
            CreateSubmissionDto(orgRef: "100001", orgName: "CSO Legacy Ltd", orgType: "compliance", journeyType: "CsoLegacy"),
            CreateSubmissionDto(orgRef: "100002", orgName: "CSO Large Producer Ltd", orgType: "compliance", journeyType: "CsoLargeProducer"),
            CreateSubmissionDto(orgRef: "100003", orgName: "CSO Small Producer Ltd", orgType: "compliance", journeyType: "CsoSmallProducer"),
            CreateSubmissionDto(orgRef: "100004", orgName: "Direct Large Producer Corp", orgType: "large", journeyType: "DirectLargeProducer"),
            CreateSubmissionDto(orgRef: "100005", orgName: "Direct Small Producer Ltd", orgType: "small", journeyType: "DirectSmallProducer"),
        ]);

        var filter = new GetOrganisationRegistrationSubmissionsFilter
        {
            NationId = 1,
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/organisation-registration-submissions", filter);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>>(JsonOptions);
        result.Should().NotBeNull();
        result!.items.Should().HaveCount(5);

        using (new AssertionScope())
        {
            result.items[0].OrganisationName.Should().Be("CSO Legacy Ltd");
            result.items[0].RegistrationJourneyType.Should().Be(RegistrationJourneyType.CsoLegacy);

            result.items[1].OrganisationName.Should().Be("CSO Large Producer Ltd");
            result.items[1].RegistrationJourneyType.Should().Be(RegistrationJourneyType.CsoLargeProducer);

            result.items[2].OrganisationName.Should().Be("CSO Small Producer Ltd");
            result.items[2].RegistrationJourneyType.Should().Be(RegistrationJourneyType.CsoSmallProducer);

            result.items[3].OrganisationName.Should().Be("Direct Large Producer Corp");
            result.items[3].RegistrationJourneyType.Should().Be(RegistrationJourneyType.DirectLargeProducer);

            result.items[4].OrganisationName.Should().Be("Direct Small Producer Ltd");
            result.items[4].RegistrationJourneyType.Should().Be(RegistrationJourneyType.DirectSmallProducer);
        }
    }

    private static object CreateSubmissionDto(string orgRef, string orgName, string orgType, string journeyType) =>
        new
        {
            submissionId = Guid.NewGuid().ToString(),
            organisationId = Guid.NewGuid().ToString(),
            organisationReference = orgRef,
            organisationName = orgName,
            organisationType = orgType,
            registrationJourney = journeyType,
            applicationReferenceNumber = $"REG-2025-{orgRef}",
            registrationReferenceNumber = (string?)null,
            submittedDateTime = "2025-01-10T09:30:00Z",
            relevantYear = 2025,
            nationId = 1,
            submissionStatus = "Pending",
            resubmissionStatus = (string?)null,
            statusPendingDate = "2025-01-10T09:30:00Z",
            isResubmission = false,
            resubmissionDate = (string?)null,
            registrationDate = (string?)null,
            regulatorDecisionDate = (string?)null,
            regulatorUserId = (string?)null,
        };

    private void SetupCommonDataApiMock(object[] data)
    {
        CommonDataApiServer.Given(Request.Create()
                .UsingGet()
                .WithPath(new WireMock.Matchers.WildcardMatcher("/api/submissions/organisation-registrations/*", true)))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(new
                {
                    items = data,
                    currentPage = 1,
                    pageSize = 20,
                    totalItems = data.Length,
                })));
    }

    private void SetupLastSyncTimeMock()
    {
        CommonDataApiServer.Given(Request.Create()
                .UsingGet()
                .WithPath("/api/submission-events/get-last-sync-time"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(new { lastSyncTime = "2025-01-27T00:00:00Z" })));
    }
}
