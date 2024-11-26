using System.ComponentModel.DataAnnotations;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.API.Controllers;

[Route("api")]
public class OrganisationRegistrationSubmissionsController(
    IOrganisationRegistrationSubmissionService organisationRegistrationHelper,
    ILogger<OrganisationRegistrationSubmissionsController> logger, 
    IMessagingService messagingService) : Controller
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Route("organisation-registration-submission-decision")]
    public async Task<IActionResult> CreateRegulatorSubmissionDecisionEvent(
        [FromBody] RegulatorDecisionCreateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            var serviceResult =
                await organisationRegistrationHelper.HandleCreateRegulatorDecisionSubmissionEvent(request,
                    GetUserId(request.UserId));

            if (serviceResult.IsSuccessStatusCode)
            { 
                SendEventEmail(request);
                return Created();
            }

            logger.LogError("Cannot create submission event: {StatusCode} with message {Message}",
                serviceResult.StatusCode, serviceResult.Content);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Exception during {nameof(CreateRegulatorSubmissionDecisionEvent)}");
            return Problem($"Exception occurred processing {nameof(CreateRegulatorSubmissionDecisionEvent)}");
        }

        return Problem();
    }

    private void SendEventEmail(RegulatorDecisionCreateRequest request)
    {
        var model = new OrganisationRegistrationSubmissionEmailModel
        { 
            ToEmail = request.OrganisationEmail,  // This is a sinle email address only.
            ApplicationNumber = request.ApplicationReferenceNumber,
            OrganisationNumber = request.OrganisationId.ToString(),
            OrganisationName = request.OrganisationName,
            Period = $"20{request.TwoDigitYear}",
            Agency = request.AgencyName,
            AgencyEmail = request.AgencyEmail,
            IsWelsh = request.IsWelsh,
        };

        switch (request.Status)
        {
            case Core.Enums.RegistrationSubmissionStatus.Refused: 
            case Core.Enums.RegistrationSubmissionStatus.Granted: 
                messagingService.OrganisationRegistrationSubmissionDecision(model); //Send same email
                break;
            case Core.Enums.RegistrationSubmissionStatus.Queried: 
                messagingService.OrganisationRegistrationSubmissionQueried(model);
                break; 
            case Core.Enums.RegistrationSubmissionStatus.Cancelled:  // dont need to send emails  
            default: // dont need to send emails
                break;
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Route("organisation-registration-fee-payment")]
    public async Task<IActionResult> CreateRegistrationFeePaymentEvent(
        [FromBody] RegistrationFeePaymentCreateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            var serviceResult =
                await organisationRegistrationHelper.HandleCreateRegistrationFeePaymentSubmissionEvent(request,
                    GetUserId(request.UserId));

            if (serviceResult.IsSuccessStatusCode)
            {
                return Created();
            }

            logger.LogError("Cannot create submission event: {StatusCode} with message {Message}",
                serviceResult.StatusCode, serviceResult.Content);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Exception during {nameof(CreateRegulatorSubmissionDecisionEvent)}");
            return Problem($"Exception occured processing {nameof(CreateRegulatorSubmissionDecisionEvent)}");
        }

        return Problem();
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Route("organisation-registration-submissions")]
    public async Task<IActionResult> GetRegistrationSubmissionList(
        [FromBody, Required] GetOrganisationRegistrationSubmissionsFilter filter)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            var result = await organisationRegistrationHelper.HandleGetRegistrationSubmissionList(filter);

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Exception during {nameof(GetRegistrationSubmissionList)}");
            return Problem($"Exception occured processing {nameof(GetRegistrationSubmissionList)}",
                HttpContext.Request.Path,
                StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Route("organisation-registration-submission-details/{submissionId:Guid}")]
    public async Task<IActionResult> GetRegistrationSubmissionDetails([Required] Guid submissionId)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            var result =
                await organisationRegistrationHelper.HandleGetOrganisationRegistrationSubmissionDetails(submissionId);

            if (null == result)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Exception during {nameof(GetRegistrationSubmissionDetails)}");
            return Problem($"Exception occured processing {nameof(GetRegistrationSubmissionDetails)}",
                HttpContext.Request.Path,
                StatusCodes.Status500InternalServerError);
        }
    }

    private Guid GetUserId(Guid? defaultId)
    {
        if (defaultId is null)
        {
            return User.UserId();
        }

        return (Guid)defaultId;
    }
}