using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;
using System.Threading.Tasks;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData;

public interface ICommonDataService
{
    Task<HttpResponseMessage> GetSubmissionLastSyncTime();

    Task<HttpResponseMessage> GetPoMSubmissions(GetPomSubmissionsRequest pomSubmissionsRequest);

    Task<HttpResponseMessage> GetRegistrationSubmissions(GetRegistrationSubmissionsRequest registrationSubmissionsRequest);

    Task<RegistrationSubmissionOrganisationDetailsFacadeResponse> GetOrganisationRegistrationSubmissionDetails(
        Guid submissionId,
        int lateFeeCutOffDay,
        int lateFeeCutOffMonth);

    Task<PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>> GetOrganisationRegistrationSubmissionList(GetOrganisationRegistrationSubmissionsFilter filter);

    Task<PomResubmissionPaycalParametersDto?> GetPomResubmissionPaycalDetails(Guid submissionId, Guid? complianceSchemeId);

    Task<PaycalParametersResponse> GetPaycalParametersAsync(
        Guid submissionId,
        IDictionary<string, string> queryParams);

    Task<SubmissionDetailsResponse> GetOrganisationRegistrationSubmissionDetailsPartAsync(
        Guid submissionId);

    Task<SubmissionStatusResponse> GetOrganisationRegistrationSubmissionStatusPartAsync(
        Guid submissionId);
}