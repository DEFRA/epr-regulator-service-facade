using System.Text.Json;
using System.Web;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models;
using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Organisations;
using EPR.RegulatorService.Facade.Core.Models.Requests;
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

            if (!request.NominationDecision)
            {
                var response = await _producerService.RemoveApprovedUser(request);
            
                if (response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<AssociatedPersonResults[]>(
                        stringContent,
                        new JsonSerializerOptions {PropertyNameCaseInsensitive = true});

                    if (result.Length > 0)
                    {
                        SendNotificationEmails(result);
                    }

                    return Ok(NoContent());
                }
                return HandleError.HandleErrorWithStatusCode(response.StatusCode);
            }
            
            return Problem("Error thrown as Nomination Decision is passed as Yes", statusCode: StatusCodes.Status400BadRequest);
            
        }
        catch (Exception e)
        {
            _logger.LogError(e,"Error deleting approved user for organisation {organisationId}", request.OrganisationId);
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
            _logger.LogInformation($@"Email sent to Invited new approved person. 
                                   Organisation external Id: {request.OrganisationId}
                                   User: {request.InvitedPersonFirstName} {request.InvitedPersonLastName}");
            
            foreach (var demotedBasicUser in addRemoveApprovedUserResponse.DemotedBasicUsers)
            {
                demotedBasicUser.TemplateId =  _messagingConfig.DemotedDelegatedUserTemplateId;
                _messagingService.SendRemovedApprovedPersonNotification(demotedBasicUser, 3);
                _logger.LogInformation($@"Email sent to demoted basic user 
                                       Organisation external Id: {request.OrganisationId}
                                       User: {demotedBasicUser.FirstName} {demotedBasicUser.LastName}
                                       Email: {demotedBasicUser.Email}");
            }
            
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, @$"Error inviting new approved person. 
                                Organisation external Id: {request.OrganisationId}
                                Invited user: {request.InvitedPersonFirstName} {request.InvitedPersonLastName}
                                Invited by user email: {invitedByUserEmail}");
            
            return BadRequest("Failed to add / remove user");
        }
    }
    
    private void SendNotificationEmails(AssociatedPersonResults[] result)
    {
        // send email for removed AP
        SendRemovalEmail(result, 1);

        // send email for demoted DP
        SendRemovalEmail(result, 3 );
    }
    private void SendRemovalEmail(AssociatedPersonResults[] emailList,int serviceRoleId)
    {
        foreach (var email in emailList.Where(a => a.ServiceRoleId == serviceRoleId).ToList())
        {
            email.TemplateId = serviceRoleId == 1 ? _messagingConfig.RemovedApprovedUserTemplateId : _messagingConfig.DemotedDelegatedUserTemplateId;
           
            var emailSent = SendNotificationEmailToDeletedPerson(email,serviceRoleId);
            
            if (!emailSent)
            {
                var errorMessage = $"Error sending the notification email to user {email.FirstName } {email.LastName} " +
                                   $" for company {email.CompanyName}";
                _logger.LogError(errorMessage);
            }
        }
    }

    private bool SendNotificationEmailToDeletedPerson(AssociatedPersonResults email,int serviceRoleId)
    {
        return _messagingService.SendRemovedApprovedPersonNotification(email, serviceRoleId) != null;
    }
}