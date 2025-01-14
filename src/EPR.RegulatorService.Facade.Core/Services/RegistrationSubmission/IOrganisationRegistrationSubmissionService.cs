using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;

namespace EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;

public interface IOrganisationRegistrationSubmissionService
{
    Task<PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>> HandleGetRegistrationSubmissionList(GetOrganisationRegistrationSubmissionsCommonDataFilter filter, Guid userId);

    Task<RegistrationSubmissionOrganisationDetailsResponse?> HandleGetOrganisationRegistrationSubmissionDetails(Guid submissionId, Guid userId);

    Task<HttpResponseMessage> HandleCreateRegulatorDecisionSubmissionEvent(RegulatorDecisionCreateRequest request, Guid userId);

    Task<HttpResponseMessage> HandleCreateRegistrationFeePaymentSubmissionEvent(RegistrationFeePaymentCreateRequest request, Guid userId);

    string GenerateReferenceNumber(
        CountryName countryName,
        RegistrationSubmissionType registrationSubmissionType,
        string applicationReferenceNumber,
        string organisationId,
        string twoDigitYear = null,
        MaterialType materialType = MaterialType.None);
}