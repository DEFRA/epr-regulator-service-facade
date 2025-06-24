using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            else if (request.IsResubmission
                    && (string.IsNullOrWhiteSpace(request.ExistingRegRefNumber)
                        ||
                        (string.IsNullOrWhiteSpace(request.FileId)) && request.Status != Core.Enums.RegistrationSubmissionStatus.Cancelled))
            {
                if (string.IsNullOrWhiteSpace(request.ExistingRegRefNumber))
                    ModelState.AddModelError(nameof(request.ExistingRegRefNumber), "ExistingRegRefNumber is required for resubmission");
                if (string.IsNullOrWhiteSpace(request.FileId))
                    ModelState.AddModelError(nameof(request.ExistingRegRefNumber), "FileId is required for resubmission");

                return ValidationProblem(ModelState);
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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Route("organisation-packaging-data-resubmission-fee-payment")]
    public async Task<IActionResult> CreatePackagingDataResubmissionFeePaymentEvent(
    [FromBody] PackagingDataResubmissionFeePaymentCreateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            var serviceResult =
                await organisationRegistrationSubmissionService.HandleCreatePackagingDataResubmissionFeePaymentEvent(request, GetUserId(request.UserId));

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

            var result = await organisationRegistrationSubmissionService.HandleGetRegistrationSubmissionList(filter, User.UserId());

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Exception during {nameof(GetRegistrationSubmissionList)}");
            return Problem($"Exception occured processing {nameof(GetRegistrationSubmissionList)}",
                HttpContext?.Request?.Path,
                StatusCodes.Status500InternalServerError);
        }
    }

    [ExcludeFromCodeCoverage]
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
                await organisationRegistrationSubmissionService.HandleGetOrganisationRegistrationSubmissionDetails(submissionId, User.UserId());

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (HttpProtocolException ex)
        {
            logger.LogError(ex, $"HttpProtocolException: Submission with ID {submissionId} causes Http Exceptions.");
            throw;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, $"HttpRequest Exception: Submission with ID {submissionId} causes Http Exceptions.");
            throw;
        }
            catch (Exception ex)
        {
            logger.LogError(ex, $"Exception during {nameof(GetRegistrationSubmissionDetails)}");
            return Problem($"Exception occured processing {nameof(GetRegistrationSubmissionDetails)}",
                HttpContext.Request.Path,
                StatusCodes.Status500InternalServerError);
        }}

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
                case Core.Enums.RegistrationSubmissionStatus.Granted: // Same email for both accept and reject
                    if (request.IsResubmission)
                    {
                        messagingService.OrganisationRegistrationResubmissionDecision(model);
                    }
                    else
                    {
                        messagingService.OrganisationRegistrationSubmissionDecision(model);
                    }
                    break;
                case Core.Enums.RegistrationSubmissionStatus.Queried:
                    messagingService.OrganisationRegistrationSubmissionQueried(model);
                    break;
                case Core.Enums.RegistrationSubmissionStatus.Cancelled:  // dont need to send emails  
                default: // dont need to send emails
                    break;
            }
        }
        catch (Exception ex)
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