using System.Net;
using Asp.Versioning;
using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}")]
[FeatureGate(FeatureFlags.ReprocessorExporter)]
public class AccreditationsController(
    IRegistrationService registrationService,
    ILogger<AccreditationsController> logger) : ControllerBase
{
    [HttpGet("registrations/{id:Guid}/accreditations")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
            Summary = "get accreditation data for the given registration.",
            Description = "Returns all accreditation data for a given site registration, including material-level and site-level tasks.  "
        )]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> GetAccreditationsByRegistrationId(Guid id)
    {
        logger.LogInformation("Get accreditation data for the given registration: {id}", id);
        var accreditations = await registrationService.GetAccreditationsByRegistrationId(id);

        return Ok(accreditations);
    }
}
