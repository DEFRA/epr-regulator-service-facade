using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData.SubmissionDetails;

namespace EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;

public interface IOrganisationRegistrationSubmissionService
{
    Task<PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>> HandleGetRegistrationSubmissionList(GetOrganisationRegistrationSubmissionsFilter filter, Guid userId);

    Task<HttpResponseMessage> HandleCreateRegulatorDecisionSubmissionEvent(RegulatorDecisionCreateRequest request, Guid userId);

    Task<HttpResponseMessage> HandleCreateRegistrationFeePaymentSubmissionEvent(RegistrationFeePaymentCreateRequest request, Guid userId);

    string GenerateReferenceNumber(
        CountryName countryName,
        RegistrationSubmissionType registrationSubmissionType,
        string applicationReferenceNumber,
        string organisationId,
        string twoDigitYear = null,
        MaterialType materialType = MaterialType.None);

    Task<HttpResponseMessage> HandleCreatePackagingDataResubmissionFeePaymentEvent(PackagingDataResubmissionFeePaymentCreateRequest request, Guid userId);

    Task<OrganisationRegistrationSubmissionDetailsResponse?> HandleGetOrganisationRegistrationSubmissionDetails(
        Guid submissionId,
        OrganisationType organisationType,
        Guid userId,
        IDictionary<string, string> queryParams);
}