using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using Azure;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Helpers;
using EPR.RegulatorService.Facade.Core.Helpers.TestData;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;
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

    public async Task<RegistrationSubmissionOrganisationDetails> GetOrganisationRegistrationSubmissionDetails(Guid submissionId)
    {
        var url = $"{_config.Endpoints.GetOrganisationRegistrationSubmissionDetails}{submissionId}";

        httpClient.Timeout = TimeSpan.FromSeconds(300);
        var response = await httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();

        var jsonObject = JsonSerializer.Deserialize<OrganisationRegistrationDetailsDto>(content, _deserialisationOptions);

        return ConvertCommonDataDetailToFEData(jsonObject);

        //return await RegistrationSubmissionTestData.GetRegistrationSubmissionDetails(submissionId, url);
    }

    [ExcludeFromCodeCoverage]
    public async Task<PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>> GetOrganisationRegistrationSubmissionList(GetOrganisationRegistrationSubmissionsFilter filter)
    {
        var url = $"{_config.Endpoints.GetOrganisationRegistrationSubmissionsSummaries}{filter.NationId}";

        url = $"{url}?{GenerateQueryString(filter)}";

        httpClient.Timeout = TimeSpan.FromSeconds(300);
        var response = await httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();

        var jsonObject = JsonSerializer.Deserialize<PaginatedResponse<OrganisationRegistrationSummaryDto>>(content, _deserialisationOptions);

        return ConvertCommonDataCollectionToFEData(jsonObject);

        //GetFromJsonAsync<PaginatedResponse<OrganisationRegistrationSummaryDto>>(url);

        //var filteredRegistrations =
        //    LocalPaginationHelper.FilterAndOrder(RegistrationSubmissionTestData.DummyData, filter);
        
        //return LocalPaginationHelper.Paginate<OrganisationRegistrationSubmissionSummaryResponse>(
        //    filteredRegistrations.Item2,
        //    filter.PageNumber.Value,
        //    filter.PageSize.Value, filteredRegistrations.Item1);
    }

    private PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse> ConvertCommonDataCollectionToFEData(PaginatedResponse<OrganisationRegistrationSummaryDto>? commonDataPaginatedCollection)
    {
        return new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>()
        {
            items = commonDataPaginatedCollection.items.Select(x => (OrganisationRegistrationSubmissionSummaryResponse)x).ToList(),
            totalItems = commonDataPaginatedCollection.totalItems,
            currentPage = commonDataPaginatedCollection.currentPage,
            pageSize = commonDataPaginatedCollection.pageSize
        };
    }

    private RegistrationSubmissionOrganisationDetails ConvertCommonDataDetailToFEData(OrganisationRegistrationDetailsDto? jsonObject)
    {
        return jsonObject == null ? null : (RegistrationSubmissionOrganisationDetails)jsonObject;
    }

    private static string GenerateQueryString(GetOrganisationRegistrationSubmissionsFilter source)
    {
        Func<string?, string?> convertSpaceToComma = input =>
           string.IsNullOrWhiteSpace(input)
               ? null
               : string.Join(",", input.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        var queryParams = new Dictionary<string, string?>
        {
            { "OrganisationNameCommaSeparated", convertSpaceToComma(source.OrganisationName) },
            { "OrganisationIDCommaSeparated", convertSpaceToComma(source.OrganisationReference) },
            { "RelevantYearCommaSeparated", convertSpaceToComma(source.RelevantYears) },
            { "SubmissionStatusCommaSeparated", convertSpaceToComma(source.Statuses) },
            { "OrganisationTypesCommaSeparated", convertSpaceToComma(source.OrganisationType) },
            { "PageNumber", source.PageNumber?.ToString() },
            { "PageSize", source.PageSize?.ToString() }
        };

        // Filter out null or empty values and encode the parameters
        var queryString = string.Join("&",
            queryParams
                .Where(kv => !string.IsNullOrEmpty(kv.Value))
                .Select(kv => $"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode(kv.Value)}"));

        return queryString;
    }
}