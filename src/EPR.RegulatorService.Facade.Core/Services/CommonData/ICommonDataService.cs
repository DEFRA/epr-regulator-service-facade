using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData;

public interface ICommonDataService
{
    Task<HttpResponseMessage> GetSubmissionLastSyncTime();

    Task<HttpResponseMessage> GetPoMSubmissions(GetPomSubmissionsRequest pomSubmissionsRequest);
    
    Task<HttpResponseMessage> GetRegistrationSubmissions(GetRegistrationSubmissionsRequest registrationSubmissionsRequest);

    Task<HttpResponseMessage> GetOrganisationRegistrationSubmissions(Guid submissionId);
}