using System.Net.Http.Json;
using EPR.RegulatorService.Facade.Core.Configs;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.Services.Submissions;

public class SubmissionsService : ISubmissionService
{
    private readonly HttpClient _httpClient;
    private readonly SubmissionsApiConfig _config;

    public SubmissionsService(
        HttpClient httpClient,
        IOptions<SubmissionsApiConfig> options)
    {
        _httpClient = httpClient;
        _config = options.Value;
    }
    
    public async Task<HttpResponseMessage> CreateSubmissionEvent<T>(Guid submissionId, T request, Guid userId)
    { 
       ConfigureHttpClient(userId); 
       
       var url = string.Format($"{_config.Endpoints.CreateSubmissionEvent}", _config.ApiVersion, submissionId);
       
       return await _httpClient.PostAsJsonAsync(url, request);
    }

    public async Task<HttpResponseMessage> GetDeltaPoMSubmissions(DateTime lastSyncTime, Guid userId)
    {
        ConfigureHttpClient(userId);
        var url = string.Format($"{_config.Endpoints.GetPoMSubmissions}?LastSyncTime={lastSyncTime.ToString("yyyy-MM-ddTHH:mm:ss")}", _config.ApiVersion);
        return await _httpClient.GetAsync(url);
    }

    private void ConfigureHttpClient(Guid userId)
    {
        _httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());
    }
}