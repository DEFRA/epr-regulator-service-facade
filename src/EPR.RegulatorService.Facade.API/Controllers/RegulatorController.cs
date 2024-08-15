using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Services.Application;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using EPR.RegulatorService.Facade.Core.Models.Applications.Users;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Notify.Models;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.Core.Models.Responses;

namespace EPR.RegulatorService.Facade.API.Controllers;

public class RegulatorController :  ControllerBase
{
    private readonly ILogger<RegulatorController> _logger;
    private readonly IApplicationService _applicationService;
    private readonly IMessagingService _messagingService;

    public RegulatorController(ILogger<RegulatorController> logger,
        IApplicationService applicationService,
        IMessagingService messagingService)
    {
        _logger = logger;
        _applicationService = applicationService;
        _messagingService = messagingService;
    }

    [HttpGet]
    [Route("api/organisations/pending-applications")]
    public async Task<IActionResult> PendingApplications(int currentPage, int pageSize, string organisationName, string applicationType)
    {
        try
        {
            var userId = User.UserId();
            if (userId == default)
            {
                _logger.LogError("UserId not available");
                return Problem("UserId not available", statusCode: StatusCodes.Status500InternalServerError);
            }

            var response = await _applicationService.PendingApplications(userId, currentPage, pageSize, organisationName, applicationType);
            if (response.IsSuccessStatusCode)
            {
                var stringContent = await response.Content.ReadAsStringAsync();
                var paginatedResponse = JsonSerializer.Deserialize<PaginatedResponse<OrganisationEnrolments>>(stringContent,
                    new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
                return Ok(paginatedResponse);
            }

            _logger.LogError("Failed to fetch pending applications");
            return HandleError.HandleErrorWithStatusCode(response.StatusCode);
        }
        catch (Exception e)
        {
            _logger.LogError(e,$"Error fetching {pageSize} Pending applications for organisation {organisationName} on page {currentPage}");
            return HandleError.Handle(e);
        }
    }

    [HttpGet]
    [Route("api/organisations/{organisationId}/pending-applications")]
    public async Task<IActionResult> GetOrganisationApplications(Guid organisationId)
    {
        try
        {
            var userId = User.UserId();
            if (userId == default)
            {
                _logger.LogError("UserId not available");
                return Problem("UserId not available", statusCode: StatusCodes.Status500InternalServerError);
            }

            var response = await _applicationService.GetOrganisationPendingApplications(userId, organisationId);
            if (response.IsSuccessStatusCode)
            {
                var stringContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApplicationEnrolmentDetailsResponse>(stringContent,
                    new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
                return Ok(result);
            }

            _logger.LogError("Failed to fetch pending applications");
            return HandleError.HandleErrorWithStatusCode(response.StatusCode);
        }
        catch (Exception e)
        {
            _logger.LogError(e,$"Error fetching applications for organisation {organisationId}");
            return HandleError.Handle(e);
        }
    }

    [HttpPost]
    [Route("api/organisations/update-enrolment")]
    public async Task<IActionResult> UpdateEnrolment([FromBody]UpdateEnrolmentRequest updateEnrolmentRequest)
    {
        try
        {
            var userId = User.UserId();
            if (userId == default)
            {
                _logger.LogError("UserId not available");
                return Problem("UserId not available", statusCode: StatusCodes.Status500InternalServerError);
            }

            var request = new ManageRegulatorEnrolmentRequest
            {
                UserId = userId,
                EnrolmentId = updateEnrolmentRequest.EnrolmentId,
                EnrolmentStatus = updateEnrolmentRequest.EnrolmentStatus,
                RegulatorComment = updateEnrolmentRequest.Comments
            };

            var response = await _applicationService.UpdateEnrolment(request);
            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }

            _logger.LogError("Failed to update enrolment");
            return HandleError.HandleErrorWithStatusCode(response.StatusCode);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error updating the enrolment {updateEnrolmentRequest.EnrolmentId} by the user {User.UserId()}");
            return HandleError.Handle(e);
        }
    }

    [HttpPost]
    [Route("api/accounts/transfer-nation")]
    public async Task<IActionResult>TransferOrganisationNation([FromBody]OrganisationTransferNationRequest request)
    {
        try
        {
            var userId = User.UserId();
            if (userId == default)
            {
                _logger.LogError("UserId not available");
                return Problem("UserId not available", statusCode: StatusCodes.Status500InternalServerError);
            }

            request.UserId = userId;
            var response = await _applicationService.TransferOrganisationNation(request);
            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }

            _logger.LogError("Failed to update enrolment");
            return HandleError.HandleErrorWithStatusCode(response.StatusCode);
        }
        catch (Exception e)
        {
            _logger.LogError(e,$"Error transferring the organisation {request.OrganisationId} to {request.TransferNationId} by the user {User.UserId()}");
            return HandleError.Handle(e);
        }
    }
    
    [HttpGet]
    [Route("api/user-accounts")]
    public async Task<IActionResult> GetUserDetails()
    {
        try
        {
            var userId = User.UserId();
            if (userId == Guid.Empty)
            {
                _logger.LogError($"Unable to get the OId for the user when attempting to get organisation details");
                return Problem("UserId not available", statusCode: StatusCodes.Status500InternalServerError);
            }
            var response = await _applicationService.GetUserOrganisations(userId);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Fetched the organisations list successfully for the user {userId}", userId);
                return Ok(response.Content.ReadFromJsonAsync<UserOrganisationsListModel>().Result);
            }
            else
            {
                _logger.LogError("Failed to fetch the organisations list for the user {userId}", userId);
                return HandleError.HandleErrorWithStatusCode(response.StatusCode);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching the organisations list for the user");
            return HandleError.Handle(e);
        }
    }

    [HttpPost]
    [Route("api/accounts/manage-user-changes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AcceptOrRejectUserDetailsChangeRequest([FromBody] ManageUserDetailsChangeRequest request)
    {
        try
        {
            var response = await _applicationService.AcceptOrRejectUserDetailsChangeRequestAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadFromJsonAsync<RegulatorUserDetailsUpdateResponse>();
                if (responseContent.HasUserDetailsAcceptedOrRejected && responseContent.ChangeHistory != null)
                {
                    _logger.LogInformation($"Accept Or Reject User Details Change Request for externalId {request.ChangeHistoryExternalId} is success");
                    try
                    {
                        var ch = responseContent.ChangeHistory;
                        var notifyEmailInput = new UserDetailsChangeNotificationEmailInput()
                        {
                            Nation = ch.Nation,
                            ExternalIdReference = ch.ExternalId,
                            ContactEmailAddress = ch.EmailAddress,
                            ContactTelephone = ch.Telephone,
                            OrganisationName = ch.OrganisationName,
                            OrganisationNumber = ch.OrganisationNumber,
                            NewFirstName = ch.NewValues.FirstName,
                            NewLastName = ch.NewValues.LastName,
                            NewJobTitle = ch.NewValues.JobTitle,
                            OldFirstName = ch.OldValues.FirstName,
                            OldLastName = ch.OldValues.LastName,
                            OldJobTitle = ch.OldValues.JobTitle
                        };

                        var notificationId = _messagingService.SendAcceptRejectUserDetailChangeEmailToEprUser(notifyEmailInput);

                        _logger.LogInformation("Send accept reject user Detail change notification email {notificationId} to EprUser sent successfully", notificationId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"failed to send Accept Reject User Detail Change Email To for  for externalId {request.ChangeHistoryExternalId}");
                    }
                }
                return Ok(responseContent);
            }
            else
            {
                _logger.LogError($"falied to Accept Or Reject User Details Change Request for externalId {request.ChangeHistoryExternalId}");
                return HandleError.HandleErrorWithStatusCode(response.StatusCode);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError($"Error in Accept Or Reject User Details Change Request for externalId {request.ChangeHistoryExternalId}");
            return HandleError.Handle(exception);
        }
    }
}
