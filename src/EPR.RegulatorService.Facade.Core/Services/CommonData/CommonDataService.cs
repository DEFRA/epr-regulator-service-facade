using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Extensions;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData;

public class CommonDataService(
    HttpClient httpClient,
    IOptions<CommonDataApiConfig> options)
    : ICommonDataService
{
    private readonly CommonDataApiConfig _config = options.Value;
    private readonly JsonSerializerOptions _deserialisationOptions = new() { PropertyNameCaseInsensitive = true };

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

    public async Task<RegistrationSubmissionOrganisationDetailsResponse> GetOrganisationRegistrationSubmissionDetails(Guid submissionId)
    {
        var url = $"{_config.Endpoints.GetOrganisationRegistrationSubmissionDetails}/{submissionId}";
     
        httpClient.Timeout = TimeSpan.FromSeconds(300);
        var response = await httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();

        var jsonObject = JsonSerializer.Deserialize<OrganisationRegistrationDetailsDto>(content, _deserialisationOptions);

        return ConvertCommonDataDetailToFEData(jsonObject);
    }

    [ExcludeFromCodeCoverage]
    public async Task<PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>> GetOrganisationRegistrationSubmissionList(GetOrganisationRegistrationSubmissionsFilter filter)
    {
        var url = $"{_config.Endpoints.GetOrganisationRegistrationSubmissionsSummaries}/{filter.NationId}?{filter.GenerateQueryString()}";

        httpClient.Timeout = TimeSpan.FromSeconds(300);
        var response = await httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();

        var jsonObject = JsonSerializer.Deserialize<PaginatedResponse<OrganisationRegistrationSummaryDto>>(content, _deserialisationOptions);

        return ConvertCommonDataCollectionToFEData(jsonObject);
    }

    private static PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse> ConvertCommonDataCollectionToFEData(PaginatedResponse<OrganisationRegistrationSummaryDto>? commonDataPaginatedCollection) => new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>()
    {
        items = commonDataPaginatedCollection.items.Select(x => (OrganisationRegistrationSubmissionSummaryResponse)x).ToList(),
        totalItems = commonDataPaginatedCollection.totalItems,
        currentPage = commonDataPaginatedCollection.currentPage,
        pageSize = commonDataPaginatedCollection.pageSize
    };

    private static RegistrationSubmissionOrganisationDetailsResponse ConvertCommonDataDetailToFEData(OrganisationRegistrationDetailsDto? jsonObject) => jsonObject == null ? null : (RegistrationSubmissionOrganisationDetailsResponse)jsonObject;
}