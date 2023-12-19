namespace EPR.RegulatorService.Facade.Core.Services.Submissions;

public interface ISubmissionService
{
    Task<HttpResponseMessage> CreateSubmissionEvent<T>(Guid submissionId, T request, Guid userId);
    
    Task<HttpResponseMessage> GetDeltaPoMSubmissions(DateTime lastSyncTime, Guid userId);
}