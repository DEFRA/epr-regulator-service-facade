using System.Text.Json;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData;
using FluentAssertions;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Models;

[TestClass]
public class OrganisationRegistrationDetailsDtoTests
{
    private static readonly JsonSerializerOptions DeserialisationOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [TestMethod]
    public void ImplicitOperator_Maps_NumberOfSubsidiariesClosedLoopRecycling_ToFacadeResponse()
    {
        var dto = new OrganisationRegistrationDetailsDto
        {
            SubmissionId = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid(),
            OrganisationName = "Test",
            OrganisationReference = "100001",
            ApplicationReferenceNumber = "REG-2025-001",
            SubmissionStatus = "Granted",
            SubmittedDateTime = "2025-01-11T10:30:00Z",
            SubmissionPeriod = "2025",
            RelevantYear = 2025,
            ResubmissionStatus = "Pending",
            IsComplianceScheme = false,
            OrganisationSize = "Large",
            OrganisationType = "Direct",
            RegistrationJourney = "LargeProducer",
            NationId = 1,
            NationCode = "GB-ENG",
            NumberOfSubsidiariesClosedLoopRecycling = 7
        };

        var response = (RegistrationSubmissionOrganisationDetailsFacadeResponse)dto;

        response.NumberOfSubsidiariesClosedLoopRecycling.Should().Be(7);
    }

    [TestMethod]
    public void ImplicitOperator_Maps_NumberOfHoldingCompaniesClosedLoopRecycling_ToFacadeResponse()
    {
        var dto = new OrganisationRegistrationDetailsDto
        {
            SubmissionId = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid(),
            OrganisationName = "Test",
            OrganisationReference = "100001",
            ApplicationReferenceNumber = "REG-2025-001",
            SubmissionStatus = "Granted",
            SubmittedDateTime = "2025-01-11T10:30:00Z",
            SubmissionPeriod = "2025",
            RelevantYear = 2025,
            ResubmissionStatus = "Pending",
            IsComplianceScheme = false,
            OrganisationSize = "Large",
            OrganisationType = "Direct",
            RegistrationJourney = "LargeProducer",
            NationId = 1,
            NationCode = "GB-ENG",
            NumberOfHoldingCompaniesClosedLoopRecycling = 2
        };

        var response = (RegistrationSubmissionOrganisationDetailsFacadeResponse)dto;

        response.NumberOfHoldingCompaniesClosedLoopRecycling.Should().Be(2);
    }

    [TestMethod]
    public void Deserialize_FromCommonDataApi_Maps_NumberOfHoldingCompaniesClosedLoopRecycling_FromJson()
    {
        const string json = """
            {
              "submissionId": "1b2c3d4e-5f6a-7b8c-9d0e-1f2a3b4c5d6e",
              "organisationId": "2c3d4e5f-6a7b-8c9d-0e1f-2a3b4c5d6e7f",
              "organisationName": "Test Org",
              "organisationReference": "100001",
              "applicationReferenceNumber": "REG-2025-001",
              "submissionStatus": "Granted",
              "submittedDateTime": "2025-01-11T10:30:00Z",
              "isLateSubmission": false,
              "numberOfHoldingCompaniesClosedLoopRecycling": 2,
              "submissionPeriod": "2025",
              "relevantYear": 2025,
              "isResubmission": false,
              "resubmissionStatus": "Pending",
              "isComplianceScheme": false,
              "organisationSize": "Large",
              "organisationType": "Direct",
              "registrationJourney": "LargeProducer",
              "nationId": 1,
              "nationCode": "GB-ENG"
            }
            """;

        var dto = JsonSerializer.Deserialize<OrganisationRegistrationDetailsDto>(json, DeserialisationOptions);

        dto.Should().NotBeNull();
        dto!.NumberOfHoldingCompaniesClosedLoopRecycling.Should().Be(2);
    }

    [TestMethod]
    public void Deserialize_FromCommonDataApi_Maps_NumberOfSubsidiariesClosedLoopRecycling_FromJson()
    {
        const string json = """
            {
              "submissionId": "1b2c3d4e-5f6a-7b8c-9d0e-1f2a3b4c5d6e",
              "organisationId": "2c3d4e5f-6a7b-8c9d-0e1f-2a3b4c5d6e7f",
              "organisationName": "Test Org",
              "organisationReference": "100001",
              "applicationReferenceNumber": "REG-2025-001",
              "submissionStatus": "Granted",
              "submittedDateTime": "2025-01-11T10:30:00Z",
              "isLateSubmission": false,
              "numberOfSubsidiariesClosedLoopRecycling": 4,
              "submissionPeriod": "2025",
              "relevantYear": 2025,
              "isResubmission": false,
              "resubmissionStatus": "Pending",
              "isComplianceScheme": false,
              "organisationSize": "Large",
              "organisationType": "Direct",
              "registrationJourney": "LargeProducer",
              "nationId": 1,
              "nationCode": "GB-ENG"
            }
            """;

        var dto = JsonSerializer.Deserialize<OrganisationRegistrationDetailsDto>(json, DeserialisationOptions);

        dto.Should().NotBeNull();
        dto!.NumberOfSubsidiariesClosedLoopRecycling.Should().Be(4);
    }
}
