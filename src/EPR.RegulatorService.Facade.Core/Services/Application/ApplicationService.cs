using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Helpers;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        var url = string.Format($"{_config.Endpoints.PendingApplications}", userId, currentPage, pageSize, organisationName, applicationType);

        var uriBuilder = new UriBuilder(_httpClient.BaseAddress)
        {
            Path = url
        };
        
        return await _httpClient.GetAsync(uriBuilder.Path);
    }

    public async Task<HttpResponseMessage> GetOrganisationPendingApplications(Guid userId, Guid organisationId)
    {
        var url = string.Format($"{_config.Endpoints.GetOrganisationsApplications}", userId, organisationId);

        _logger.LogInformation("Attempting to fetch applications for the organisation {organisationId}", organisationId);

        var uriBuilder = new UriBuilder(_httpClient.BaseAddress)
        {
            Path = url
        };

        return await _httpClient.GetAsync(uriBuilder.Path);
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

        var logData = $"Attempting to fetch the organisations for user {userId}";
        _logger.LogInformation("{Message}", logData);

        return await _httpClient.GetAsync(url);
    }
}
