using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Services.RegistrationSubmissions;
using Microsoft.AspNetCore.Mvc;

namespace EPR.RegulatorService.Facade.API.Controllers;

[Route("api")]
public class OrganisationRegistrationController(IRegistrationSubmissionService registrationSubmissionService) : Controller
{
    private readonly IRegistrationSubmissionService _registrationSubmissionService = registrationSubmissionService;

    [HttpPost]
    [Route("registration/registration-submission-decision")]
    public async Task<IActionResult> CreateRegistrationSubmissionDecisionEvent([FromBody] RegistrationSubmissionDecisionCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        var registrationSubmissionEvent = await _registrationSubmissionService.CreateAsync(
            new RegistrationSubmissionDecisionEvent
            {
                OrganisationId = request.OrganisationId,
                SubmissionId = request.SubmissionId,
                Decision = request.Decision,
                RegulatorComment = request.RegulatorComment
            },
            User.UserId(),
            request.SubmissionId);

        registrationSubmissionEvent.EnsureSuccessStatusCode();

        //TODO:: what to do with registrationSubmissionEvent and the exception/error scenario?

        return Ok();
    }
}