using System.ComponentModel.DataAnnotations;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;
using Microsoft.AspNetCore.Mvc;

namespace EPR.RegulatorService.Facade.API.Controllers;

[Route("api")]
public class OrganisationRegistrationSubmissionsController(
    IOrganisationRegistrationSubmissionService organisationRegistrationSubmissionService,
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
                await organisationRegistrationSubmissionService.HandleCreateRegulatorDecisionSubmissionEvent(request,
                    GetUserId(request.UserId));

            if (serviceResult.IsSuccessStatusCode)
            {
                SendEventEmail(request);
                return Created();
            }

            return await Problem(serviceResult);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Exception during {nameof(CreateRegulatorSubmissionDecisionEvent)}");
            return Problem($"Exception occurred processing {nameof(CreateRegulatorSubmissionDecisionEvent)}");
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
                await organisationRegistrationSubmissionService.HandleCreateRegistrationFeePaymentSubmissionEvent(request,
                    GetUserId(request.UserId));

            if (serviceResult.IsSuccessStatusCode)
            {
                return Created();
            }

            return await Problem(serviceResult);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Exception during {nameof(CreateRegulatorSubmissionDecisionEvent)}");
            return Problem($"Exception occured processing {nameof(CreateRegulatorSubmissionDecisionEvent)}");
        }
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

            var result = await organisationRegistrationSubmissionService.HandleGetRegistrationSubmissionList(filter);

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
                await organisationRegistrationSubmissionService.HandleGetOrganisationRegistrationSubmissionDetails(submissionId);

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

    private void SendEventEmail(RegulatorDecisionCreateRequest request)
    {
        try
        {
            var model = new OrganisationRegistrationSubmissionEmailModel
            {
                ToEmail = request.OrganisationEmail,
                ApplicationNumber = request.ApplicationReferenceNumber,
                OrganisationNumber = request.OrganisationAccountManagementId.ToString(), // This is not the OrgID value
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
        catch(Exception ex)
        {
            logger.LogError(ex, $"Exception during {nameof(SendEventEmail)}");
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
    private async Task<IActionResult> Problem(HttpResponseMessage serviceResult)
    {
        if (serviceResult.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var validationProblemDetails = await serviceResult.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            return ValidationProblem(validationProblemDetails);
        }

        var problemDetails = await serviceResult.Content.ReadFromJsonAsync<ProblemDetails>();
        return Problem(statusCode: problemDetails.Status, detail: problemDetails.Detail);
    }
}