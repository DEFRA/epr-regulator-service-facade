using System.Net.Http.Json;

namespace IntegrationTests.Features;

using System.Text.Json;
using FluentAssertions;
using IntegrationTests.Infrastructure;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

[Collection("Sequential")] // Shared mock common data can't be used safely in parallel
public class OrganisationRegistrationSubmissionsTests : IntegrationTestBase
{
    [Fact]
    public async Task GetRegistrationSubmissionList_ReturnsSuccess_WithValidFilter()
    {
        // Arrange
        var submissionId = Guid.Parse("0163A629-7780-445F-B00E-1898546BDF0C");
        var organisationId = Guid.Parse("EE29CFAE-81AB-435F-8759-7285959530DB");
        
        var mockData = new[]
        {
            new
            {
                submissionId = submissionId.ToString(),
                organisationId = organisationId.ToString(),
                organisationReference = "100001",
                organisationName = "Compliance Scheme Ltd",
                organisationType = "compliance",
                registrationJourney = "CsoLargeProducer",
                submissionStatus = "Pending",
                statusPendingDate = "2025-01-10T09:30:00Z",
                applicationReferenceNumber = "REG-2025-001",
                registrationReferenceNumber = (string?)null,
                relevantYear = 2025,
                submittedDateTime = "2025-01-10T09:30:00Z",
                regulatorDecisionDate = (string?)null,
                regulatorUserId = (string?)null,
                nationId = 1,
                isResubmission = false,
                resubmissionStatus = (string?)null,
                resubmissionDate = (string?)null,
                registrationDate = (string?)null
            }
        };

        SetupCommonDataMockOrganisationRegistrations(mockData, nationId: 1);

        var filter = new
        {
            nationId = 1,
            pageNumber = 1,
            pageSize = 20
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/organisation-registration-submissions", filter);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        result.GetProperty("items").GetArrayLength().Should().Be(1);
        result.GetProperty("totalItems").GetInt32().Should().Be(1);
    }

    [Fact]
    public async Task GetRegistrationSubmissionDetails_ReturnsSuccess_WithValidSubmissionId()
    {
        // Arrange
        var submissionId = Guid.Parse("0163A629-7780-445F-B00E-1898546BDF0C");
        var organisationId = Guid.Parse("EE29CFAE-81AB-435F-8759-7285959530DB");

        var mockData = new
        {
            submissionId = submissionId.ToString(),
            organisationId = organisationId.ToString(),
            organisationName = "Compliance Scheme Ltd",
            organisationReference = "100001",
            applicationReferenceNumber = "REG-2025-001",
            registrationReferenceNumber = (string?)null,
            submissionStatus = "Pending",
            statusPendingDate = "2025-01-10T09:30:00Z",
            submittedDateTime = "2025-01-10T09:30:00Z",
            isLateSubmission = true,
            submissionPeriod = "January to December 2025",
            relevantYear = 2025,
            isResubmission = false,
            resubmissionStatus = "Pending",
            registrationDate = (string?)null,
            resubmissionDate = (string?)null,
            resubmissionFileId = (string?)null,
            isComplianceScheme = true,
            organisationSize = (string?)null,
            organisationType = "compliance",
            registrationJourney = "CsoLargeProducer",
            nationId = 1,
            nationCode = "GB-ENG",
            regulatorComment = "",
            producerComment = "Test comment",
            regulatorDecisionDate = (string?)null,
            producerCommentDate = (string?)null,
            regulatorResubmissionDecisionDate = (string?)null,
            regulatorUserId = (string?)null,
            companiesHouseNumber = "CS123456",
            buildingName = "Test Building",
            subBuildingName = "1",
            buildingNumber = "1",
            street = "Test Street",
            locality = "Test Locality",
            dependentLocality = "2",
            town = "Test Town",
            county = "Test County",
            country = "United Kingdom",
            postcode = "SW1A 1AA",
            submittedUserId = "A1B2C3D4-E5F6-7890-ABCD-EF1234567890",
            firstName = "Test",
            lastName = "User",
            email = "test.user@example.com",
            telephone = "01234567890",
            serviceRole = "Approved Person",
            serviceRoleId = 1,
            isOnlineMarketPlace = false,
            numberOfSubsidiaries = 0,
            numberOfOnlineSubsidiaries = 0,
            companyDetailsFileId = "F1A2B3C4-D5E6-7890-ABCD-EF1234567890",
            companyDetailsFileName = "reg-20250110_093000.csv",
            companyDetailsBlobName = "B1C2D3E4-F5A6-7890-ABCD-EF1234567890",
            partnershipFileId = (string?)null,
            partnershipFileName = (string?)null,
            partnershipBlobName = (string?)null,
            brandsFileId = (string?)null,
            brandsFileName = (string?)null,
            brandsBlobName = (string?)null,
            csoJson = "[{\"memberId\":\"100001\",\"memberType\":\"large\",\"isOnlineMarketPlace\":false,\"isLateFeeApplicable\":true,\"numberOfSubsidiaries\":0,\"NumberOfSubsidiariesOnlineMarketPlace\":0,\"relevantYear\":2025,\"submittedDate\":\"2025-01-10T07:24:00\",\"submissionPeriodDescription\":\"January to December 2025\"}]"
        };

        SetupCommonDataMockOrganisationRegistrationDetails(submissionId, mockData);

        // Act
        var response = await Client.GetAsync($"/api/organisation-registration-submission-details/{submissionId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        result.GetProperty("submissionId").GetString().Should().Be(submissionId.ToString());
        result.GetProperty("organisationName").GetString().Should().Be("Compliance Scheme Ltd");
    }

    [Fact]
    public async Task GetRegistrationSubmissionDetails_ReturnsNotFound_WithInvalidSubmissionId()
    {
        // Arrange
        var submissionId = Guid.NewGuid();

        // Mock empty response (CommonDataService returns null for empty content, which causes controller to return NotFound)
        CommonDataServer.Given(Request.Create()
                .UsingGet()
                .WithPath($"/api/submissions/organisation-registration-submission/{submissionId}"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(""));

        // Act
        var response = await Client.GetAsync($"/api/organisation-registration-submission-details/{submissionId}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}
