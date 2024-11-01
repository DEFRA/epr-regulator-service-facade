using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Enums; 
using EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;
using Microsoft.AspNetCore.Mvc; 

namespace EPR.RegulatorService.Facade.API.Controllers;

public class RegistrationSubmissionController : ControllerBase
{
    private readonly ILogger<RegistrationSubmissionController> _logger;
    private readonly IRegistrationSubmissionService _registrationSubmissionService; 

    public RegistrationSubmissionController(ILogger<RegistrationSubmissionController> logger, IRegistrationSubmissionService registrationSubmissionService)
    {
        _logger = logger;
        _registrationSubmissionService = registrationSubmissionService;
    }

    [HttpGet]
    [Route("api/registration-submission/generate-reference-number")]
    public async Task<IActionResult> GenerateReferenceNumber(CountryName countryName, RegistrationSubmissionType registrationSubmissionType, string organisationId, string twoDigitYear = null)
    {
        try
        {
            var referenceNumber = _registrationSubmissionService.GenerateReferenceNumber(countryName,registrationSubmissionType,organisationId,twoDigitYear);
            return Ok(referenceNumber);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
            return HandleError.Handle(e);
        }
    }
}