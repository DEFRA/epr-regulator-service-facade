using System.Globalization;
using System.Net.Http.Json;
using EPR.RegulatorService.Facade.Core.Configs;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.Services.Submissions;

public class SubmissionsService(
    HttpClient httpClient,
    IOptions<SubmissionsApiConfig> options) : ISubmissionService
{
    private readonly SubmissionsApiConfig _config = options.Value;

    public async Task<HttpResponseMessage> CreateSubmissionEvent<T>(Guid submissionId, T request, Guid userId)
    {
        ConfigureHttpClient(userId);

        var url = string.Format($"{_config.Endpoints.CreateSubmissionEvent}", _config.ApiVersion, submissionId);

        var response = await httpClient.PostAsJsonAsync(url, request);

        return response;
    }

    public async Task<HttpResponseMessage> GetDeltaPoMSubmissions(DateTime lastSyncTime, Guid userId)
    {
        ConfigureHttpClient(userId);

        var url = string.Format($"{_config.Endpoints.GetPoMSubmissions}?LastSyncTime={lastSyncTime.ToString("yyyy-MM-ddTHH:mm:ss")}", _config.ApiVersion);

        return await httpClient.GetAsync(url);
    }

    public async Task<HttpResponseMessage> GetDeltaRegistrationSubmissions(DateTime lastSyncTime, Guid userId)
    {
        ConfigureHttpClient(userId);

        var url = string.Format($"{_config.Endpoints.GetRegistrationSubmissions}?LastSyncTime={lastSyncTime.ToString("yyyy-MM-ddTHH:mm:ss")}", _config.ApiVersion);

        return await httpClient.GetAsync(url);
    }

    public async Task<HttpResponseMessage> GetDeltaOrganisationRegistrationEvents(DateTime lastSyncTime, Guid userId, Guid? SubmissionId)
    {
        ConfigureHttpClient(userId);

        var url = string.Format($"{_config.Endpoints.GetOrganisationRegistrationEvents}?LastSyncTime={lastSyncTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)}", _config.ApiVersion);

        if ( SubmissionId.HasValue )
        {
            url += "&SubmissionId={SubmissionId}";
        }

        return await httpClient.GetAsync(url);
    }

    private void ConfigureHttpClient(Guid userId)
    {
        httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());
    }
}