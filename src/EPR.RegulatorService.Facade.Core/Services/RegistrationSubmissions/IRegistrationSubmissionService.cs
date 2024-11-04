namespace EPR.RegulatorService.Facade.Core.Services.RegistrationSubmissions;

public interface IRegistrationSubmissionService
{
    Task<HttpResponseMessage> CreateAsync<T>(T request, Guid userId, Guid submissionId);
}