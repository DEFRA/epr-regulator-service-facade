using System.Diagnostics.Eventing.Reader;
using System.Drawing.Printing;
using System.Text.Json;
using System.Web;
using Azure;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Helpers;
using EPR.RegulatorService.Facade.Core.Models;
using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Organisations;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions;
using EPR.RegulatorService.Facade.Core.Models.Responses;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.Core.Services.Producer;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using Microsoft.AspNetCore.Http;
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
            var logData = $"Error fetching {pageSize} organisations by {searchTerm} on page {currentPage}";
            _logger.LogError(logData, e);

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

            string logData = $"Fetching organisation details for {externalId} resulted in unsuccessful request: {response.StatusCode}";
            _logger.LogInformation(logData);

            return HandleError.HandleErrorWithStatusCode(response.StatusCode);
        }
        catch (Exception e)
        {
            string logData = $"Error fetching organisation details for {externalId}";
            _logger.LogError(logData, e);

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
            string logData = $"Error fetching producer organisations by external organisation id {externalId}";
            _logger.LogError(logData, e);

            return HandleError.Handle(e);
        }
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("api/organisations/remove-approved-users")]
    public async Task<IActionResult> RemoveApprovedPerson(RemoveApprovedUsersRequest request)
    {
        try
        {
            request.UserId = User.UserId();
           
            if (request.UserId == default)
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
                    new JsonSerializerOptions {PropertyNameCaseInsensitive = true});

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
            string logData = $"Error deleting approved user for organisation {request.OrganisationId}";
            _logger.LogError(logData, e);
            return HandleError.Handle(e);
        }
    }
    
    [HttpPost]
    [Route("api/organisations/add-remove-approved-users")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
            if (invitedByUserId == default)
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

            string logData = String.Format("Email sent to Invited new approved person. Organisation external Id: {0} User: {1} {2}",
                                request.OrganisationId,
                                request.InvitedPersonFirstName,
                                request.InvitedPersonLastName);

            _logger.LogInformation(logData);

            // Send email to Demoted users.
            var emailUser = addRemoveApprovedUserResponse.AssociatedPersonList.Where(r => !string.IsNullOrWhiteSpace(r.FirstName) && !string.IsNullOrWhiteSpace(r.LastName)).ToArray<AssociatedPersonResults>();
            SendRemovalEmail(emailUser);
            return Ok();
        }
        catch (Exception e)
        {
            string logData = @$"Error inviting new approved person. 
                                Organisation external Id: {request.OrganisationId}
                                Invited user: {request.InvitedPersonFirstName} {request.InvitedPersonLastName}
                                Invited by user email: {invitedByUserEmail}";
            _logger.LogError(logData, e);
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
                var errorMessage = $"Error sending the notification email to user {email.FirstName } {email.LastName} " +
                                   $" for company {email.CompanyName}";
                _logger.LogError(errorMessage);
            }
         
        }
    }

    private bool SendNotificationEmailToDeletedPerson(AssociatedPersonResults email,string notificationType)
    {
        return _messagingService.SendRemovedApprovedPersonNotification(email, notificationType) != null;
    }
}