using System.Net.Http.Json;
using System.Text;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Helpers;
using EPR.RegulatorService.Facade.Core.Helpers.TestData;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;
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

    public async Task<HttpResponseMessage> GetOrganisationRegistrationSubmissionDetails(Guid submissionId)
    {
        var url = string.Format($"{_config.Endpoints.GetOrganisationRegistrationSubmissionDetails}");

        return await RegistrationSubmissionTestData.GetRegistrationSubmissionDetails(submissionId, url);
    }

    public async Task<HttpResponseMessage> GetOrganisationRegistrationSubmissionlist(GetOrganisationRegistrationSubmissionsFilter filter)
    {
        //var url = string.Format($"{_config.Endpoints.GetOrganisationRegistrationSubmissions}");

        var filteredRegistrations = LocalPaginationHelper.FilterAndOrder(RegistrationSubmissionTestData.DummyData, filter);

        var paginatedRegistrations = LocalPaginationHelper.Paginate<RegistrationSubmissionOrganisationDetails>(
            filteredRegistrations.Item2,
            filter.PageNumber.Value,
            filter.PageSize.Value);

        return new HttpResponseMessage(System.Net.HttpStatusCode.Created)
        {
            Content = new StringContent(
                paginatedRegistrations,
                Encoding.UTF8,
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"))
        };
    }
}