using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;
using EPR.RegulatorService.Facade.Core.Services.CommonData.ConcreteData;
using EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

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

    public async Task<HttpResponseMessage> GetOrganisationRegistrations<T>(GetOrganisationRegistrationRequest organisationRegistrationRequest) where T : IOrganisationRegistrationDataSource
    {
        IOrganisationRegistrationDataSource handler = typeof(T) switch
        {
            var type when type == typeof(CommonDataOrganisationRegistrationHandler) =>
                new CommonDataOrganisationRegistrationHandler(_httpClient, _config),

            var type when type == typeof(JsonOrganisationRegistrationHandler) =>
                new JsonOrganisationRegistrationHandler("Services\\CommonData\\DummyData\\dummyregistrationdata.json", new OrganisationDummyDataLoader()),

            _ => throw new InvalidOperationException($"Unknown handler type ${nameof(T)})")
        };

        return await handler.GetOrganisationRegistrations(organisationRegistrationRequest);
    }
}
