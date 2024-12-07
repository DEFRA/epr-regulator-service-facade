using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;
using System.Threading.Tasks;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData;

public interface ICommonDataService
{
    Task<HttpResponseMessage> GetSubmissionLastSyncTime();

    Task<HttpResponseMessage> GetPoMSubmissions(GetPomSubmissionsRequest pomSubmissionsRequest);

    Task<HttpResponseMessage> GetRegistrationSubmissions(GetRegistrationSubmissionsRequest registrationSubmissionsRequest);

    Task<RegistrationSubmissionOrganisationDetails> GetOrganisationRegistrationSubmissionDetails(Guid submissionId);

    Task<PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>> GetOrganisationRegistrationSubmissionList(GetOrganisationRegistrationSubmissionsFilter filter);
}