namespace IntegrationTests.Features;

using System.Text.Json;
using FluentAssertions;
using IntegrationTests.Infrastructure;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

[Collection("Sequential")] // Shared mock common data can't be used safely in parallel
public class SubmissionsTests : IntegrationTestBase
{
    [Fact]
    public async Task GetPoMSubmissions_ReturnsSuccess_WithValidRequest()
    {
        // Arrange
        var lastSyncTime = DateTime.UtcNow.AddDays(-1);
        SetupCommonDataMockLastSyncTime(lastSyncTime);

        // Mock empty delta decisions from submissions API (GET request with query parameter)
        CommonDataServer.Given(Request.Create()
                .UsingGet()
                .WithPath("/api/v1/submissions/events/get-regulator-pom-decision")
                .WithParam("LastSyncTime"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("[]"));

        var mockPoMData = new[]
        {
            new
            {
                submissionId = Guid.NewGuid().ToString(),
                organisationId = Guid.NewGuid().ToString(),
                organisationName = "Test Organisation",
                organisationReference = "100001",
                organisationType = "large",
                submissionYear = 2025,
                submissionPeriod = "January to December 2025",
                submissionStatus = "Pending",
                submittedDate = "2025-01-10T09:30:00Z"
            }
        };

        SetupCommonDataMockPoMSubmissions(mockPoMData);

        // Act - Add query parameters as required by the endpoint
        var response = await Client.GetAsync("/api/pom/get-submissions?PageNumber=1&PageSize=20");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        result.GetProperty("items").GetArrayLength().Should().Be(1);
        result.GetProperty("totalItems").GetInt32().Should().Be(1);
    }

    [Fact]
    public async Task GetRegistrationSubmissions_ReturnsSuccess_WithValidRequest()
    {
        // Arrange
        var lastSyncTime = DateTime.UtcNow.AddDays(-1);
        SetupCommonDataMockLastSyncTime(lastSyncTime);

        // Mock empty delta decisions from submissions API (GET request with query parameter)
        CommonDataServer.Given(Request.Create()
                .UsingGet()
                .WithPath("/api/v1/submissions/events/get-regulator-registration-decision")
                .WithParam("LastSyncTime"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("[]"));

        var mockRegistrationData = new[]
        {
            new
            {
                submissionId = Guid.NewGuid().ToString(),
                organisationId = Guid.NewGuid().ToString(),
                organisationName = "Test Organisation",
                organisationReference = "100001",
                organisationType = "large",
                submissionYear = 2025,
                submissionPeriod = "January to December 2025",
                submissionStatus = "Pending",
                submittedDate = "2025-01-10T09:30:00Z"
            }
        };

        SetupCommonDataMockRegistrationSubmissions(mockRegistrationData);

        // Act - Add required query parameters (PageNumber and PageSize are required)
        var response = await Client.GetAsync("/api/registrations/get-submissions?PageNumber=1&PageSize=20");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        result.GetProperty("items").GetArrayLength().Should().Be(1);
        result.GetProperty("totalItems").GetInt32().Should().Be(1);
    }

    [Fact]
    public async Task GetResubmissionPaycalDetails_ReturnsSuccess_WithValidSubmissionId()
    {
        // Arrange
        var submissionId = Guid.NewGuid();
        var mockPaycalData = new
        {
            isResubmission = false,
            resubmissionDate = (string?)null,
            memberCount = (int?)null,
            reference = (string?)null,
            nationCode = (string?)null,
            referenceNotAvailable = false,
            referenceFieldNotAvailable = false
        };

        SetupCommonDataMockPomResubmissionPaycalParameters(submissionId, mockPaycalData);

        // Act
        var response = await Client.GetAsync($"/api/pom/get-resubmission-paycal-parameters/{submissionId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        
        // Verify the response contains expected properties from PomResubmissionPaycalParametersDto
        result.GetProperty("referenceNotAvailable").GetBoolean().Should().BeFalse();
        result.GetProperty("referenceFieldNotAvailable").GetBoolean().Should().BeFalse();
    }

    [Fact]
    public async Task GetResubmissionPaycalDetails_ReturnsNoContent_WhenDataNotFound()
    {
        // Arrange
        var submissionId = Guid.NewGuid();

        CommonDataServer.Given(Request.Create()
                .UsingGet()
                .WithPath($"/api/submissions/pom-resubmission-paycal-parameters/{submissionId}"))
            .RespondWith(Response.Create()
                .WithStatusCode(204));

        // Act
        var response = await Client.GetAsync($"/api/pom/get-resubmission-paycal-parameters/{submissionId}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }
}
