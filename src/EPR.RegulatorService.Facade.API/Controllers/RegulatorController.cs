using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Services.Application;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using EPR.RegulatorService.Facade.Core.Models.Applications.Users;

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
            if (userId == Guid.Empty)
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
            if (userId == Guid.Empty)
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
            if (userId == Guid.Empty)
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
            if (userId == Guid.Empty)
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
}
