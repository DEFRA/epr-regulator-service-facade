using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;
using System.Threading.Tasks;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData.SubmissionDetails;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData;

public interface ICommonDataService
{
    Task<HttpResponseMessage> GetSubmissionLastSyncTime();

    Task<HttpResponseMessage> GetPoMSubmissions(GetPomSubmissionsRequest pomSubmissionsRequest);

    Task<HttpResponseMessage> GetRegistrationSubmissions(GetRegistrationSubmissionsRequest registrationSubmissionsRequest);

    Task<PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>> GetOrganisationRegistrationSubmissionList(GetOrganisationRegistrationSubmissionsFilter filter);

    Task<PomResubmissionPaycalParametersDto?> GetPomResubmissionPaycalDetails(Guid submissionId, Guid? complianceSchemeId);

    Task<PaycalParametersDto> GetProducerPaycalParametersAsync(
        Guid submissionId,
        IDictionary<string, string> lateFeeRules);

    Task<List<PaycalParametersDto>> GetCsoPaycalParametersAsync(
        Guid submissionId,
        IDictionary<string, string> lateFeeRules);

    Task<SubmissionDetailsDto> GetOrganisationRegistrationSubmissionDetailsAsync(
        Guid submissionId);

}