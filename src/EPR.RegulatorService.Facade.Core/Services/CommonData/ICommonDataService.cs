using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData;

public interface ICommonDataService
{
    Task<HttpResponseMessage> GetSubmissionLastSyncTime();

    Task<HttpResponseMessage> GetPoMSubmissions(PoMSubmissionsGetRequest request);
}