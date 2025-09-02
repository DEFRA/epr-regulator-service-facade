using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Http.Json;

namespace EPR.RegulatorService.Facade.Core.Services.Application;

public class ApplicationService : IApplicationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApplicationService> _logger;
    private readonly AccountsServiceApiConfig _config;

    public ApplicationService(
        HttpClient httpClient,
        ILogger<ApplicationService> logger,
        IOptions<AccountsServiceApiConfig> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _config = options.Value;
    }

    public async Task<HttpResponseMessage> PendingApplications(Guid userId, int currentPage, int pageSize, string organisationName, string applicationType)
    {
        _logger.LogInformation("Attempting to fetch pending applications from the backend");

        var endpoint = _config.Endpoints.PendingApplications;
        var url = BuildPendingApplicationsUrl(endpoint, userId, currentPage, pageSize, organisationName, applicationType);
        return await _httpClient.GetAsync(url);
    }

    public async Task<HttpResponseMessage> GetOrganisationPendingApplications(Guid userId, Guid organisationId)
    {
        var url = string.Format($"{_config.Endpoints.GetOrganisationsApplications}", userId, organisationId);

        _logger.LogInformation("Attempting to fetch applications for the organisation {OrganisationId}", organisationId);

        return await _httpClient.GetAsync(url);
    }

    public async Task<HttpResponseMessage> UpdateEnrolment(ManageRegulatorEnrolmentRequest request)
    {
        var url = string.Format($"{_config.Endpoints.ManageEnrolment}");

        _logger.LogInformation("User {UserId} attempting to update the enrolment {EnrolmentId}", request.UserId, request.EnrolmentId);

        return await _httpClient.PostAsJsonAsync(url, request);
    }

    public async Task<HttpResponseMessage> TransferOrganisationNation(OrganisationTransferNationRequest request)
    {
        var url = string.Format($"{_config.Endpoints.TransferOrganisationNation}");

        _logger.LogInformation("User {UserId} attempting to transfer the organisation {OrganisationId} to {NationName}",
             request.UserId, request.OrganisationId, request.TransferNationId);

        return await _httpClient.PostAsJsonAsync(url, request);
    }

    public async Task<HttpResponseMessage> GetUserOrganisations(Guid userId)
    {
        var url = string.Format($"{_config.Endpoints.UserOrganisations}?userId={userId}");

        _logger.LogInformation("Attempting to fetch the organisations for user '{UserId}'", userId);

        return await _httpClient.GetAsync(url);
    }

    private static string BuildPendingApplicationsUrl(
        string endpoint, Guid userId, int currentPage, int pageSize, string? organisationName, string? applicationType)
    {
        var url = endpoint;
        url = url.Replace("{0}", Uri.EscapeDataString(userId.ToString()), StringComparison.Ordinal);
        url = url.Replace("{1}", Uri.EscapeDataString(currentPage.ToString(CultureInfo.InvariantCulture)), StringComparison.Ordinal);
        url = url.Replace("{2}", Uri.EscapeDataString(pageSize.ToString(CultureInfo.InvariantCulture)), StringComparison.Ordinal);
        url = url.Replace("{3}", Uri.EscapeDataString(organisationName ?? string.Empty), StringComparison.Ordinal);
        url = url.Replace("{4}", Uri.EscapeDataString(applicationType ?? string.Empty), StringComparison.Ordinal);
        return url;
    }
}
