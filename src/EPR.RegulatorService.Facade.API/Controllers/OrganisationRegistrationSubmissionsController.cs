using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;

namespace EPR.RegulatorService.Facade.API.Controllers;

[Route("api")]
public class OrganisationRegistrationSubmissionsController(
    IOrganisationRegistrationSubmissionService orgService,
    ISubmissionService submissionsService,
    ICommonDataService commonDataService,
    ILogger<OrganisationRegistrationSubmissionsController> logger,
    IRegistrationSubmissionService submissionService) : Controller
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

        var regRefNumber = request.Status == RegistrationStatus.Granted ?
                                                submissionService.GenerateReferenceNumber(request.CountryName, request.RegistrationSubmissionType, request.OrganisationAccountManagementId.ToString(), request.TwoDigitYear)
                                                : string.Empty;

        var registrationSubmissionEvent = await submissionsService.CreateSubmissionEvent(
            request.SubmissionId,
            new RegistrationSubmissionDecisionEvent
            {
                OrganisationId = request.OrganisationId,
                SubmissionId = request.SubmissionId,
                Decision = request.Status.GetRegulatorDecision(),
                Comments = request.Comments,
                RegistrationReferenceNumber = regRefNumber
            },
            User.UserId()
        );

        if (registrationSubmissionEvent.IsSuccessStatusCode)
        {
            return Created();
        }

        logger.LogWarning("Cannot create submission event");
        return Problem();
    }

    [HttpGet]
    [Route("organisation-registration-submission-details/submissionId/{submissionId:GUID}")]
    public async Task<IActionResult> GetRegistrationSubmissionDetails([Required] Guid submissionId)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        var registrationSubmissionDetailsResponse = await commonDataService.GetOrganisationRegistrationSubmissionDetails(submissionId);

        if (registrationSubmissionDetailsResponse.IsSuccessStatusCode)
        {
            var stringContent = await registrationSubmissionDetailsResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<RegistrationSubmissionOrganisationDetails>(stringContent, jsonSerializerOptions);
            return Ok(response);
        }

        return HandleError.HandleErrorWithStatusCode(registrationSubmissionDetailsResponse.StatusCode);
    }

    [HttpPost]
    [Route("organisation-registration-submissions-list")]
    public async Task<IActionResult> GetRegistrationSubmissionList([FromQuery, Required] GetOrganisationRegistrationSubmissionsFilter filter)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        return await orgService.GetOrganisationRegistrations(filter);
    }
}