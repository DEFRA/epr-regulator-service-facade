namespace EPR.RegulatorService.Facade.Core.Services.RegistrationSubmissions;

public interface IRegistrationSubmissionsService
{
    Task<HttpResponseMessage> CreateRegulatorDecisionEventAsync<T>(Guid submissionId, Guid userId, T request);
}