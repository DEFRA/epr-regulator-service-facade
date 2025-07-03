using System.Text.Json;
using System.Web;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Extensions;
using EPR.RegulatorService.Facade.Core.Models;
using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Organisations;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions;
using EPR.RegulatorService.Facade.Core.Models.Responses;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.Core.Services.Producer;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.API.Controllers;

[ApiController]
public class OrganisationsSearchController : ControllerBase
{
    private readonly ILogger<OrganisationsSearchController> _logger;
    private readonly IProducerService _producerService;
    private readonly IRegulatorOrganisationService _regulatorOrganisationService;
    private readonly MessagingConfig _messagingConfig;
    private readonly IMessagingService _messagingService;
    private readonly JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    
    public OrganisationsSearchController(
        ILogger<OrganisationsSearchController> logger,
        IProducerService producerService,
        IRegulatorOrganisationService regulatorOrganisationService,
        IOptions<MessagingConfig> messagingConfig,
        IMessagingService messagingService
        )
    {
        _logger = logger;
        _producerService = producerService;
        _regulatorOrganisationService = regulatorOrganisationService;
        _messagingConfig = messagingConfig.Value;
        _messagingService = messagingService;
    }

    [HttpGet]
    [Route("api/organisations/search-organisations")]
    [ProducesResponseType(typeof(PaginatedResponse<OrganisationSearchResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrganisationsBySearchTerm(int currentPage, int pageSize, string searchTerm)
    {
        try
        {
            var userId = User.UserId();

            if (!userId.IsValidGuid())
            {
                _logger.LogError("UserId not available");
                return Problem("UserId not available", statusCode: StatusCodes.Status500InternalServerError);
            }
            
            var response = await _producerService.GetOrganisationsBySearchTerm(userId, currentPage, pageSize, searchTerm);
            if (response.IsSuccessStatusCode)
            {
                var stringContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PaginatedResponse<OrganisationSearchResult>>(stringContent,
                    options);
                return Ok(result);
            }
            
            _logger.LogError("Failed to fetch organisations");
            return HandleError.HandleErrorWithStatusCode(response.StatusCode);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching {PageSize} organisations by {SearchTerm} on page {CurrentPage}", pageSize, searchTerm, currentPage);

            return HandleError.Handle(e);
        }
    }
    
    [HttpGet]
    [Route("api/organisations/organisation-details")]
    [ProducesResponseType(typeof(OrganisationDetailResults), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> OrganisationDetails(Guid externalId)
    {
        try
        {
            var userId = User.UserId();
            
            if (!userId.IsValidGuid())
            {
                _logger.LogError("UserId not available");
                return Problem("UserId not available", statusCode: StatusCodes.Status500InternalServerError);
            }

            var response = await _producerService.GetOrganisationDetails(userId, externalId);
            if (response.IsSuccessStatusCode)
            {
                var stringContent = await response.Content.ReadAsStringAsync();
                
                var organisationDetails = JsonSerializer.Deserialize<OrganisationDetailResults>(stringContent,
                    options);
                return Ok(organisationDetails);
            }

            _logger.LogError("Fetching organisation details for {ExternalId} resulted in unsuccessful request: {StatusCode}", externalId, response.StatusCode);

            return HandleError.HandleErrorWithStatusCode(response.StatusCode);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching organisation details for {ExternalId}", externalId);
            return HandleError.Handle(e);
        }
    }
    
    [HttpGet]
    [Route("api/organisations/users-by-organisation-external-id")]
    [ProducesResponseType(typeof(List<OrganisationUserOverviewResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsersByOrganisationExternalId(Guid externalId)
    {
        try
        {
            var userId = User.UserId();
            
            if (!userId.IsValidGuid())
            {
                _logger.LogError("UserId not available");
                return Problem("UserId not available", statusCode: StatusCodes.Status500InternalServerError);
            }
            
            var response = await _regulatorOrganisationService.GetUsersByOrganisationExternalId(userId, externalId);
            if (response.IsSuccessStatusCode)
            {
                var stringContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<OrganisationUserOverviewResponseModel>>(stringContent,
                    options);
                return Ok(result);
            }

            _logger.LogError("Failed to fetch organisations");
            return HandleError.HandleErrorWithStatusCode(response.StatusCode);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching producer organisations by external organisation id {ExternalId}", externalId);

            return HandleError.Handle(e);
        }
    }
    
    [HttpPost]
    [Route("api/organisations/remove-approved-users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveApprovedPerson(RemoveApprovedUsersRequest request)
    {
        try
        {
            request.UserId = User.UserId();

            var userId = User.UserId();

            if (!userId.IsValidGuid())
            {
                _logger.LogError("UserId not available");
                return Problem("UserId not available", statusCode: StatusCodes.Status500InternalServerError);
            }
            
            var response = await _producerService.RemoveApprovedUser(request);
        
            if (response.IsSuccessStatusCode)
            {
                var stringContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AssociatedPersonResults[]>(
                    stringContent,
                    options);

                if (result.Length > 0)
                {
                    result = result.Where(r => !string.IsNullOrWhiteSpace(r.FirstName) && !string.IsNullOrWhiteSpace(r.LastName)).ToArray<AssociatedPersonResults>();
                    SendRemovalEmail(result);
                }

                return Ok(NoContent());
            }
            return HandleError.HandleErrorWithStatusCode(response.StatusCode);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting approved user for organisation {OrganisationId}", request.OrganisationId);
            return HandleError.Handle(e);
        }
    }
    
    [HttpPost]
    [Route("api/organisations/add-remove-approved-users")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddRemoveApprovedUser(FacadeAddRemoveApprovedPersonRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        var invitedByUserId = User.UserId();
        var invitedByUserEmail = User.Email();
        try
        {
            if (!invitedByUserId.IsValidGuid())
            {
                _logger.LogError("UserId not available");
                return Problem("UserId not available", statusCode: StatusCodes.Status500InternalServerError);
            }

            var addRemoveApprovedUserRequest = new AddRemoveApprovedUserRequest
            {
                OrganisationId = request.OrganisationId,
                RemovedConnectionExternalId = request.RemovedConnectionExternalId,
                InvitedPersonEmail = request.InvitedPersonEmail,
                AddingOrRemovingUserEmail = invitedByUserEmail, 
                AddingOrRemovingUserId = invitedByUserId
            };
                
            var response = await _regulatorOrganisationService.AddRemoveApprovedUser(addRemoveApprovedUserRequest);

            var addRemoveApprovedUserResponse = await response.Content.ReadFromJsonAsync<AddRemoveApprovedPersonResponseModel>();
            
            var emailModel = new AddRemoveNewApprovedPersonEmailModel
            {
                Email = request.InvitedPersonEmail,
                FirstName = request.InvitedPersonFirstName,
                LastName = request.InvitedPersonLastName,
                OrganisationNumber = addRemoveApprovedUserResponse.OrganisationReferenceNumber,
                CompanyName = addRemoveApprovedUserResponse.OrganisationName,
                InviteLink =  $"{_messagingConfig.AccountCreationUrl}{HttpUtility.UrlEncode(addRemoveApprovedUserResponse.InviteToken)}"
            };
            
            _messagingService.SendEmailToInvitedNewApprovedPerson(emailModel);
            _logger.LogInformation(@"Email sent to Invited new approved person. Organisation external Id: {OrganisationId} User: {InvitedPersonFirstName} {InvitedPersonLastName}", request.OrganisationId, request.InvitedPersonFirstName, request.InvitedPersonLastName);

            // Send email to Demoted users.
            var emailUser = addRemoveApprovedUserResponse.AssociatedPersonList.Where(r => !string.IsNullOrWhiteSpace(r.FirstName) && !string.IsNullOrWhiteSpace(r.LastName)).ToArray<AssociatedPersonResults>();
            SendRemovalEmail(emailUser);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, @"Error inviting new approved person. Organisation external Id: {OrganisationId} Invited user: {InvitedPersonFirstName} {InvitedPersonLastName} Invited by user email: {InvitedByUserEmail}", request.OrganisationId, request.InvitedPersonFirstName, request.InvitedPersonLastName, invitedByUserEmail);

            return BadRequest("Failed to add / remove user");
        }
    }
    
    private void SendRemovalEmail(AssociatedPersonResults[] emailList)
    {
        foreach (var email in emailList)
        {
            // new code
            email.AccountSignInUrl = _messagingConfig.AccountSignInUrl;
            email.TemplateId = email.EmailNotificationType switch
            {
                "RemovedApprovedUser" => _messagingConfig.RemovedApprovedUserTemplateId,
                "DemotedDelegatedUsed" => _messagingConfig.DemotedDelegatedUserTemplateId,
                "PromotedApprovedUser" => _messagingConfig.PromotedApprovedUserTemplateId,
                _ => email.TemplateId
            };

            var emailSent = SendNotificationEmailToDeletedPerson(email, email.EmailNotificationType);
            if (!emailSent)
            {
                _logger.LogError("Error sending the notification email to user {FirstName} {LastName} for company {CompanyName}", email.FirstName, email.LastName, email.CompanyName);
            }
         
        }
    }

    private bool SendNotificationEmailToDeletedPerson(AssociatedPersonResults email,string notificationType)
    {
        return _messagingService.SendRemovedApprovedPersonNotification(email, notificationType) != null;
    }
}