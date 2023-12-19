using System.Net;
using System.Text.Json;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Organisations;
using EPR.RegulatorService.Facade.Core.Services.Producer;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using Microsoft.AspNetCore.Mvc;

namespace EPR.RegulatorService.Facade.API.Controllers;

[ApiController]
public class OrganisationsSearchController : ControllerBase
{
    private readonly ILogger<OrganisationsSearchController> _logger;
    private readonly IProducerService _producerService;
    private readonly IRegulatorOrganisationService _regulatorOrganisationService;

    public OrganisationsSearchController(
        ILogger<OrganisationsSearchController> logger,
        IProducerService producerService,
        IRegulatorOrganisationService regulatorOrganisationService
        )
    {
        _logger = logger;
        _producerService = producerService;
        _regulatorOrganisationService = regulatorOrganisationService;
    }

    [HttpGet]
    [Route("api/organisations/search-organisations")]
    public async Task<IActionResult> GetOrganisationsBySearchTerm(int currentPage, int pageSize, string searchTerm)
    {
        try
        {
            var userId = User.UserId();
            if (userId == default)
            {
                _logger.LogError("UserId not available");
                return Problem("UserId not available", statusCode: StatusCodes.Status500InternalServerError);
            }
            
            var response = await _producerService.GetOrganisationsBySearchTerm(userId, currentPage, pageSize, searchTerm);
            if (response.IsSuccessStatusCode)
            {
                var stringContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PaginatedResponse<OrganisationSearchResult>>(stringContent,
                    new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
                return Ok(result);
            }
            
            _logger.LogError("Failed to fetch organisations");
            return HandleError.HandleErrorWithStatusCode(response.StatusCode);
        }
        catch (Exception e)
        {
            _logger.LogError(e,$"Error fetching {pageSize} organisations by {searchTerm} on page {currentPage}");
            return HandleError.Handle(e);
        }
    }
    
    [HttpGet]
    [Route("api/organisations/organisation-details")]
    public async Task<IActionResult> OrganisationDetails(Guid externalId)
    {
        try
        {
            var userId = User.UserId();
            if (userId == default)
            {
                _logger.LogError("UserId not available");
                return Problem("UserId not available", statusCode: StatusCodes.Status500InternalServerError);
            }

            var response = await _producerService.GetOrganisationDetails(userId, externalId);
            if (response.IsSuccessStatusCode)
            {
                var stringContent = await response.Content.ReadAsStringAsync();
                var organisationDetails = JsonSerializer.Deserialize<OrganisationDetailResults>(stringContent,
                    new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
                return Ok(organisationDetails);
            }

            _logger.LogError("Fetching organisation details for {externalId} resulted in unsuccessful request: {statusCode}", externalId, response.StatusCode);
            return HandleError.HandleErrorWithStatusCode(response.StatusCode);
        }
        catch (Exception e)
        {
            _logger.LogError(e,"Error fetching organisation details for {externalId}", externalId);
            return HandleError.Handle(e);
        }
    }
    
    [HttpGet]
    [Route("api/organisations/users-by-organisation-external-id")]
    public async Task<IActionResult> GetUsersByOrganisationExternalId(Guid externalId)
    {
        try
        {
            var userId = User.UserId();
            if (userId == default)
            {
                _logger.LogError("UserId not available");
                return Problem("UserId not available", statusCode: StatusCodes.Status500InternalServerError);
            }
            
            var response = await _regulatorOrganisationService.GetUsersByOrganisationExternalId(userId, externalId);
            if (response.IsSuccessStatusCode)
            {
                var stringContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<OrganisationUserOverviewResponseModel>>(stringContent,
                    new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
                return Ok(result);
            }
            
            _logger.LogError("Failed to fetch organisations");
            return HandleError.HandleErrorWithStatusCode(response.StatusCode);
        }
        catch (Exception e)
        {
            _logger.LogError(e,$"Error fetching producer organisations by external organisation id {externalId}");
            return HandleError.Handle(e);
        }
    }
    
    [HttpDelete]
    [Route("api/organisations/remove-approved-users")]
    public async Task<IActionResult> RemoveApprovedPerson(Guid connExternalId, Guid organisationId)
    {
        try
        {
            var userId = User.UserId();
           
            if (userId == default)
            {
                _logger.LogError("UserId not available");
                return Problem("UserId not available", statusCode: StatusCodes.Status500InternalServerError);
            }

            var response = await _producerService.RemoveApprovedUser(userId, connExternalId, organisationId);
            return response.IsSuccessStatusCode ? Ok(NoContent()) : HandleError.HandleErrorWithStatusCode(response.StatusCode);
        }
        catch (Exception e)
        {
            _logger.LogError(e,"Error deleting approved user for organisation {organisationId}", organisationId);
            return HandleError.Handle(e);
        }
    }
}