using System.Net.Http.Json;
using EPR.RegulatorService.Facade.Core.Configs;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.Services.RegistrationSubmissions;

public class RegistrationSubmissionService(
    HttpClient httpClient,
    IOptions<SubmissionsApiConfig> options) : IRegistrationSubmissionService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly SubmissionsApiConfig _config = options.Value;

    public async Task<HttpResponseMessage> CreateAsync<T>(T request, Guid userId, Guid submissionId)
    { 
       ConfigureHttpClient(userId); 
       
       var url = string.Format($"{_config.Endpoints.CreateSubmissionEvent}", _config.ApiVersion, submissionId);
        
       return await _httpClient.PostAsJsonAsync(url, request);
    }

    private void ConfigureHttpClient(Guid userId)
    {
        _httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());
    }
}