using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace EPR.RegulatorService.Facade.API.Controllers;

[Route("api")]
public class OrganisationRegistrationSubmissionsController(ISubmissionService submissionsService, ICommonDataService commonDataService, ILogger<OrganisationRegistrationSubmissionsController> logger) : Controller
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    [HttpPost]
    [Route("organisation-registration-submission-decision")]
    public async Task<IActionResult> CreateRegistrationSubmissionDecisionEvent([FromBody] RegistrationSubmissionDecisionCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        var registrationSubmissionEvent = await submissionsService.CreateSubmissionEvent(
            request.SubmissionId,
            new RegistrationSubmissionDecisionEvent
            {
                OrganisationId = request.OrganisationId,
                SubmissionId = request.SubmissionId,
                Decision = request.Status.GetRegulatorDecision(),
                Comments = request.Comments
            },
            User.UserId()
        );

        if (registrationSubmissionEvent.IsSuccessStatusCode) {
            return Created();
        }

        logger.LogWarning("Cannot create submission event");
        return Problem();
    }

    [HttpGet]
    [Route("registrations-submission-details")]
    public async Task<IActionResult> GetRegistrationSubmissionDetails([FromQuery, Required] GetRegistrationSubmissionDetailsRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        var registrationSubmissionDetailsResponse = await commonDataService.GetRegistrationSubmissionDetails(request);

        if (registrationSubmissionDetailsResponse.IsSuccessStatusCode)
        {
            var stringContent = await registrationSubmissionDetailsResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<RegistrationSubmissionDetailsResponse>(stringContent, jsonSerializerOptions);
            return Ok(response);
        }

        return HandleError.HandleErrorWithStatusCode(registrationSubmissionDetailsResponse.StatusCode);
    }
}