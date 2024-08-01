using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Helpers;
using EPR.RegulatorService.Facade.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http.Json;

namespace EPR.RegulatorService.Facade.Core.Services.Producer;

public class ProducerService : IProducerService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProducerService> _logger;
    private readonly AccountsServiceApiConfig _config;

    public ProducerService(
        HttpClient httpClient,
        ILogger<ProducerService> logger,
        IOptions<AccountsServiceApiConfig> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _config = options.Value;
    }
    
    public async Task<HttpResponseMessage> GetOrganisationsBySearchTerm(Guid userId, int currentPage, int pageSize, string searchTerm)
    {
        var url = string.Format($"{_config.Endpoints.GetOrganisationsBySearchTerm}", userId, currentPage, pageSize, searchTerm);

        _logger.LogInformation("Attempting to fetch organisations by searchTerm '{searchTerm}'", searchTerm);
        
        return await _httpClient.GetAsync(UrlHelpers.CheckRequestURL(url));
    }
    
    public async Task<HttpResponseMessage> GetOrganisationDetails(Guid userId, Guid externalId)
    {
        var url = string.Format($"{_config.Endpoints.GetOrganisationDetails}", userId, externalId);

        _logger.LogInformation("Attempting to fetch organisation details for organisation'{externalId}'", externalId);

        return await _httpClient.GetAsync(UrlHelpers.CheckRequestURL(url));
    }
    public async Task<HttpResponseMessage> RemoveApprovedUser(RemoveApprovedUsersRequest model)
    {
        var url = string.Format($"{_config.Endpoints.RegulatorRemoveApprovedUser}", model.UserId, model.RemovedConnectionExternalId, model.OrganisationId, model.PromotedPersonExternalId);
        
        _logger.LogInformation("Attempting to fetch the users for organisation external id {externalId} from the backend", model.RemovedConnectionExternalId);
        
        return await _httpClient.PostAsJsonAsync(url, model);
    }
}
