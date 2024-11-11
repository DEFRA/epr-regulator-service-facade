using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Helpers;
using EPR.RegulatorService.Facade.Core.Helpers.TestData;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData;

public class CommonDataService(
    HttpClient httpClient,
    IOptions<CommonDataApiConfig> options)
    : ICommonDataService
{
    private readonly CommonDataApiConfig _config = options.Value;

    public async Task<HttpResponseMessage> GetSubmissionLastSyncTime()
    {
        return await httpClient.GetAsync(_config.Endpoints.GetSubmissionEventsLastSyncTime);
    }

    public async Task<HttpResponseMessage> GetPoMSubmissions(GetPomSubmissionsRequest pomSubmissionsRequest)
    {
        var url = string.Format($"{_config.Endpoints.GetPoMSubmissions}");

        return await httpClient.PostAsJsonAsync(url, pomSubmissionsRequest);
    }

    public async Task<HttpResponseMessage> GetRegistrationSubmissions(
        GetRegistrationSubmissionsRequest registrationSubmissionsRequest)
    {
        var url = string.Format($"{_config.Endpoints.GetRegistrationSubmissions}");
        return await httpClient.PostAsJsonAsync(url, registrationSubmissionsRequest);
    }

    public async Task<RegistrationSubmissionOrganisationDetails> GetOrganisationRegistrationSubmissionDetails(Guid submissionId)
    {
        var url = string.Format($"{_config.Endpoints.GetOrganisationRegistrationSubmissionDetails}");

        return await RegistrationSubmissionTestData.GetRegistrationSubmissionDetails(submissionId, url);
    }

    [ExcludeFromCodeCoverage]
    public async Task<PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>> GetOrganisationRegistrationSubmissionList(GetOrganisationRegistrationSubmissionsFilter filter)
    {
        var filteredRegistrations =
            LocalPaginationHelper.FilterAndOrder(RegistrationSubmissionTestData.DummyData, filter);
        
        return LocalPaginationHelper.Paginate<OrganisationRegistrationSubmissionSummaryResponse>(
            filteredRegistrations.Item2,
            filter.PageNumber.Value,
            filter.PageSize.Value, filteredRegistrations.Item1);
    }
}