using System.Net;
using Asp.Versioning;
using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.API.Filters.Swashbuckle;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version.apiVersion}")]
[FeatureGate(FeatureFlags.ReprocessorExporter)]
public class RegistrationsController(IRegistrationService registrationService
    , IValidator<UpdateTaskStatusRequestDto> updateTaskStatusValidator
    , ILogger<RegistrationsController> logger) : ControllerBase
{

    [HttpPatch("regulatorRegistrationTaskStatus/{id:int}")]
    [SwaggerOperation(
            Summary = "Updates a registration-level task (no associated material).",
            Description = "Attempting to update regulator registration task status."
        )]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [SwaggerRequestExample(typeof(UpdateTaskStatusRequestDto), typeof(UpdateTaskStatusRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(BadRequestExample))]
    public async Task<IActionResult> UpdateRegulatorRegistrationTaskStatus([FromRoute] int id, [FromBody] UpdateTaskStatusRequestDto request)
    {
        await updateTaskStatusValidator.ValidateAndThrowAsync(request);

        logger.LogInformation("Attempting to update regulator registration task status");

        _ = await registrationService.UpdateRegulatorRegistrationTaskStatus(id, request);

        return NoContent();
    }

    [HttpPatch("regulatorApplicationTaskStatus/{id:int}")]
    [SwaggerOperation(
            Summary = "Updates a material-specific task status.",
            Description = "Attempting to update regulator application task status."
        )]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [SwaggerRequestExample(typeof(UpdateTaskStatusRequestDto), typeof(UpdateTaskStatusRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(BadRequestExample))]
    public async Task<IActionResult> UpdateRegulatorApplicationTaskStatus([FromRoute] int id, [FromBody] UpdateTaskStatusRequestDto request)
    {
        await updateTaskStatusValidator.ValidateAndThrowAsync(request);

        logger.LogInformation("Attempting to update regulator application task status");

        _ = await registrationService.UpdateRegulatorApplicationTaskStatus(id, request);

        return NoContent();
    }
}
