using System.Net.Http.Json;
using EPR.RegulatorService.Facade.Core.Configs;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.Services.RegistrationSubmissions;

public class RegistrationSubmissionsService(
    HttpClient httpClient,
    IOptions<SubmissionsApiConfig> options) : IRegistrationSubmissionsService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly SubmissionsApiConfig _config = options.Value;

    public async Task<HttpResponseMessage> CreateRegulatorDecisionEventAsync<T>(Guid submissionId, Guid userId, T request)
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