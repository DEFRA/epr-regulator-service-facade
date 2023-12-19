using System.Net.Http.Json;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions;
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

    public async Task<HttpResponseMessage> GetPoMSubmissions(PoMSubmissionsGetRequest request)
    {
        var url = string.Format($"{_config.Endpoints.GetPoMSubmissions}");
        return await _httpClient.PostAsJsonAsync(url, request);
    }
}