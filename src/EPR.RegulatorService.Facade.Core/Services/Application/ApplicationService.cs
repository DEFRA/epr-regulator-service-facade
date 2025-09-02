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
        var url = string.Format(CultureInfo.InvariantCulture,endpoint,
        Uri.EscapeDataString(userId.ToString()),
        Uri.EscapeDataString(currentPage.ToString(CultureInfo.InvariantCulture)),
        Uri.EscapeDataString(pageSize.ToString(CultureInfo.InvariantCulture)),
        Uri.EscapeDataString(organisationName ?? string.Empty),
        Uri.EscapeDataString(applicationType ?? string.Empty)
        );

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
}
