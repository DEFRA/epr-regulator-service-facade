using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using Microsoft.AspNetCore.Mvc;

namespace EPR.RegulatorService.Facade.API.Controllers;

[Route("api")]
public class OrganisationRegistrationSubmissionsController(ISubmissionService submissionsService, ILogger<OrganisationRegistrationSubmissionsController> logger) : Controller
{
    private readonly ISubmissionService _submissionsService = submissionsService;
    private readonly ILogger<OrganisationRegistrationSubmissionsController> _logger = logger;

    [HttpPost]
    [Route("organisation-registration-submission-decision")]
    public async Task<IActionResult> CreateRegistrationSubmissionDecisionEvent([FromBody] RegistrationSubmissionDecisionCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        var registrationSubmissionEvent = await _submissionsService.CreateSubmissionEvent(
            request.SubmissionId,
            new RegistrationSubmissionDecisionEvent
            {
                OrganisationId = request.OrganisationId,
                SubmissionId = request.SubmissionId,
                Decision = request.Status.GetRegulatorDecision(),
                RegulatorComment = request.Comments
            },
            User.UserId()
        );

        if (registrationSubmissionEvent.IsSuccessStatusCode) {
            return Created();
        }

        _logger.LogWarning("Cannot create submission event");
        return Problem();
    }
}