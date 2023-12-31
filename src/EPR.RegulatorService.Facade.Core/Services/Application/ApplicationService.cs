using System.Net.Http.Json;
using EPR.RegulatorService.Facade.Core.Configs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EPR.RegulatorService.Facade.Core.Models.Applications;

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
        var url = string.Format($"{_config.Endpoints.PendingApplications}", userId, currentPage, pageSize, organisationName, applicationType);

        _logger.LogInformation("Attempting to fetch pending applications from the backend");

        return await _httpClient.GetAsync(url);
    }

    public async Task<HttpResponseMessage> GetOrganisationPendingApplications(Guid userId, Guid organisationId)
    {
        var url = string.Format($"{_config.Endpoints.GetOrganisationsApplications}", userId, organisationId);

        _logger.LogInformation("Attempting to fetch applications for the organisation {organisationId}", organisationId);

        return await _httpClient.GetAsync(url);
    }

    public async Task<HttpResponseMessage> UpdateEnrolment(ManageRegulatorEnrolmentRequest request)
    {
        var url = string.Format($"{_config.Endpoints.ManageEnrolment}");

        _logger.LogInformation("User {userId} attempting to update the enrolment {enrolmentId}", request.UserId,
            request.EnrolmentId);

        return await _httpClient.PostAsJsonAsync(url, request);
    }

    public async Task<HttpResponseMessage> TransferOrganisationNation(OrganisationTransferNationRequest request)
    {
        var url = string.Format($"{_config.Endpoints.TransferOrganisationNation}");

        _logger.LogInformation("User {userId} attempting to transfer the organisation {organisationId} to {NationName}",
             request.UserId, request.OrganisationId, request.TransferNationId);

        return await _httpClient.PostAsJsonAsync(url, request);
    }

    public async Task<HttpResponseMessage> GetUserOrganisations(Guid userId)
    {
        var url = string.Format($"{_config.Endpoints.UserOrganisations}?userId={userId}");

        _logger.LogInformation("Attempting to fetch the organisations for user '{userId}'", userId);

        return await _httpClient.GetAsync(url);
    }
}
