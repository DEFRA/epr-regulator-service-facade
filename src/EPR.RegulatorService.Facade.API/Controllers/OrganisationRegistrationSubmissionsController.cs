using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EPR.RegulatorService.Facade.API.Controllers;

[Route("api")]
public class OrganisationRegistrationSubmissionsController(ISubmissionService submissionsService, ILogger<OrganisationRegistrationSubmissionsController> logger) : Controller
{
    [HttpPost]
    [Route("organisation-registration-submission-decision")]
    public async Task<IActionResult> CreateRegistrationSubmissionDecisionEvent([FromBody] RegistrationSubmissionDecisionCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        var srv = new RegistrationSubmissionService();

        var registrationSubmissionEvent = await submissionsService.CreateSubmissionEvent(
            request.SubmissionId,
            new RegistrationSubmissionDecisionEvent
            {
                OrganisationId = request.OrganisationId,
                SubmissionId = request.SubmissionId,
                Decision = request.Status.GetRegulatorDecision(),
                Comments = request.Comments,
                RegistrationReferenceNumber = request.Status == RegistrationStatus.Granted ? 
                                                srv.GenerateReferenceNumber(request.CountryName,request.RegistrationSubmissionType,request.OrganisationAccountManagementId.ToString(),request.TwoDigitYear)
                                                : string.Empty
            },
            User.UserId()
        );

        if (registrationSubmissionEvent.IsSuccessStatusCode) {
            return Created();
        }

        logger.LogWarning("Cannot create submission event");
        return Problem();
    }
}