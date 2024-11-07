using System.Net.Http.Json;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Helpers.TestData;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData;

public class CommonDataService : ICommonDataService
{
    private readonly HttpClient _httpClient;
    private readonly CommonDataApiConfig _config;

    public CommonDataService(
        HttpClient httpClient,
        IOptions<CommonDataApiConfig> options)
    {
        _httpClient = httpClient;
        _config = options.Value;
    }
    
    public async Task<HttpResponseMessage> GetSubmissionLastSyncTime()
    {
        return await _httpClient.GetAsync(_config.Endpoints.GetSubmissionEventsLastSyncTime);
    }

    public async Task<HttpResponseMessage> GetPoMSubmissions(GetPomSubmissionsRequest pomSubmissionsRequest)
    {
        var url = string.Format($"{_config.Endpoints.GetPoMSubmissions}");
        
        return await _httpClient.PostAsJsonAsync(url, pomSubmissionsRequest);
    }

    public async Task<HttpResponseMessage> GetRegistrationSubmissions(GetRegistrationSubmissionsRequest registrationSubmissionsRequest)
    {
        var url = string.Format($"{_config.Endpoints.GetRegistrationSubmissions}");
        return await _httpClient.PostAsJsonAsync(url, registrationSubmissionsRequest);
    }

    public async Task<HttpResponseMessage> GetRegistrationSubmissionDetails(Guid submissionId)
    {
        var url = string.Format($"{_config.Endpoints.GetRegistrationSubmissionDetails}");

        return await RegistrationSubmissionTestData.GetRegistrationSubmissionDetails(submissionId, url);
    }
}