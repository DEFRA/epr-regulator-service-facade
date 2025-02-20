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
using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData;

public class CommonDataService(
    HttpClient httpClient,
    IOptions<CommonDataApiConfig> options,
    ILogger<CommonDataService> logger)
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
     
        var response = await httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();

        if ( string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        var jsonObject = JsonSerializer.Deserialize<OrganisationRegistrationDetailsDto>(content, _deserialisationOptions);

        return ConvertCommonDataDetailToFEData(jsonObject);
    }

    public async Task<PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>> GetOrganisationRegistrationSubmissionList(GetOrganisationRegistrationSubmissionsFilter filter)
    {
        var url = $"{_config.Endpoints.GetOrganisationRegistrationSubmissionsSummaries}/{filter.NationId}?{filter.GenerateQueryString()}";

        var response = await httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();

        if ( string.IsNullOrWhiteSpace(content))
        {
            return new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>
            {
                currentPage = 1,
                pageSize = filter.PageSize ?? 20,
                totalItems = 0,
                items = []
            };
        }

        var jsonObject = JsonSerializer.Deserialize<PaginatedResponse<OrganisationRegistrationSummaryDto>>(content, _deserialisationOptions);

        return ConvertCommonDataCollectionToFEData(jsonObject);
    }

    public async Task<PomResubmissionPaycalParametersDto?> GetPomResubmissionPaycalDetails(Guid submissionId, Guid? complianceSchemeId)
    {
        var url = $"{_config.Endpoints.GetPomResubmissionPaycalParameters}/{submissionId}";
        
        if (complianceSchemeId.HasValue)
        {
            url += $"?ComplianceSchemeId={complianceSchemeId}";
        }

        try
        {
            var response = await httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }
            var jsonObject = JsonSerializer.Deserialize<PomResubmissionPaycalParametersDto>(content, _deserialisationOptions);

            return jsonObject;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing Pom Resubmission Paycal details");
        }

        return null;
    }

    [ExcludeFromCodeCoverage]
    private static PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse> ConvertCommonDataCollectionToFEData(PaginatedResponse<OrganisationRegistrationSummaryDto>? commonDataPaginatedCollection) => new()
    {
        items = commonDataPaginatedCollection.items.Select(x => (OrganisationRegistrationSubmissionSummaryResponse)x).ToList(),
        totalItems = commonDataPaginatedCollection.totalItems,
        currentPage = commonDataPaginatedCollection.currentPage,
        pageSize = commonDataPaginatedCollection.pageSize
    };

    [ExcludeFromCodeCoverage]
    private RegistrationSubmissionOrganisationDetailsResponse ConvertCommonDataDetailToFEData(OrganisationRegistrationDetailsDto? jsonObject)
    {
        if ( jsonObject == null) return null;

        var objRet = (RegistrationSubmissionOrganisationDetailsResponse)jsonObject;

        if (!string.IsNullOrWhiteSpace(jsonObject.CSOJson))
        {
            try
            {
                List<CsoMembershipDetailsDto> csoDetails = JsonSerializer.Deserialize<List<CsoMembershipDetailsDto>>(jsonObject.CSOJson);
                objRet.CsoMembershipDetails = csoDetails;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Cannot parse the CSO Membership details JSON object");
            }
        }

        return objRet;
    }
}