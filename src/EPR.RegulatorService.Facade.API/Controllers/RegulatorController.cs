using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Services.Application;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using EPR.RegulatorService.Facade.Core.Models.Applications.Users;
using System.Drawing.Printing;
using Azure.Core;
using EPR.RegulatorService.Facade.Core.Helpers;
using Notify.Models;

namespace EPR.RegulatorService.Facade.API.Controllers;

public class RegulatorController :  ControllerBase
{
    private readonly ILogger<RegulatorController> _logger;
    private readonly IApplicationService _applicationService;

    public RegulatorController(ILogger<RegulatorController> logger, IApplicationService applicationService)
    {
        _logger = logger;
        _applicationService = applicationService;
    }

    [HttpGet]
    [Route("api/organisations/pending-applications")]
    public async Task<IActionResult> PendingApplications(int currentPage, int pageSize, string organisationName, string applicationType)
    {
        try
        {
            var userId = User.UserId();
            Guid validUserId;

            if ((!Guid.TryParse(userId.ToString(), out validUserId)) || validUserId == Guid.Empty)
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
            string logError = string.Format("Error fetching {0} Pending applications for organisation {1} on page {2}", pageSize, organisationName, currentPage).Replace('\n', '_');
            _logger.LogError(e, "{Message}", logError);
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
            Guid validUserId;

            if ((!Guid.TryParse(userId.ToString(), out validUserId)) || validUserId == Guid.Empty)
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
            string logError = $"Error fetching applications for organisation {organisationId}";
            _logger.LogError(e, "{Message}", logError.Replace('\n', '_'));
            
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
            Guid validUserId;

            if ((!Guid.TryParse(userId.ToString(), out validUserId)) || validUserId == Guid.Empty)
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
            string logError = $"Error updating the enrolment {updateEnrolmentRequest.EnrolmentId} by the user {User.UserId()}";
            _logger.LogError(e, "{Message}", logError.Replace('\n', '_'));
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
            Guid validUserId;

            if ((!Guid.TryParse(userId.ToString(), out validUserId)) || validUserId == Guid.Empty)
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
            string logError = string.Format($"Error transferring the organisation {0} to {1} by the user {2}", request.OrganisationId, request.TransferNationId, User.UserId());
            _logger.LogError(e, "{Message}", logError.Replace('\n', '_'));
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
                string logError = $"Unable to get the OId for the user when attempting to get organisation details";
                _logger.LogInformation(logError);


                return Problem("UserId not available", statusCode: StatusCodes.Status500InternalServerError);
            }
            var response = await _applicationService.GetUserOrganisations(userId);

            if (response.IsSuccessStatusCode)
            {
                string logInfo = string.Format("Fetched the organisations list successfully for the user {0}", userId);
                _logger.LogInformation(logInfo.Replace('\n', '_'));
                return Ok(response.Content.ReadFromJsonAsync<UserOrganisationsListModel>().Result);
            }
            else
            {
                string logInfo = string.Format("Failed to fetch the organisations list for the user {0}", userId);
                _logger.LogInformation("{Message}", logInfo.Replace('\n', '_'));
                return HandleError.HandleErrorWithStatusCode(response.StatusCode);
            }
        }
        catch (Exception e)
        {
            string logError = "Error fetching the organisations list for the user";
            _logger.LogError(e, "{Message}", logError.Replace('\n', '_'));
            return HandleError.Handle(e);
        }
    }
}
