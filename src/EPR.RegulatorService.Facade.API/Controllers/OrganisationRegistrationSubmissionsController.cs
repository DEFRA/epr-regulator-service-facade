using System.ComponentModel.DataAnnotations;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;
using Microsoft.AspNetCore.Mvc;

namespace EPR.RegulatorService.Facade.API.Controllers;

[Route("api")]
public class OrganisationRegistrationSubmissionsController(
    IOrganisationRegistrationSubmissionService organisationRegistrationHelper,
    ILogger<OrganisationRegistrationSubmissionsController> logger) : Controller
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
                return Created();
            }

            logger.LogError("Cannot create submission event: {StatusCode} with message {Message}",
                serviceResult.StatusCode, serviceResult.Content);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Exception during {nameof(CreateRegulatorSubmissionDecisionEvent)}");
            return Problem($"Exception occured processing {nameof(CreateRegulatorSubmissionDecisionEvent)}",
                HttpContext.Request.Path,
                StatusCodes.Status500InternalServerError);
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