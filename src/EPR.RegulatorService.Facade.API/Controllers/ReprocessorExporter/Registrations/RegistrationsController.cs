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
    private readonly IRegistrationService _registrationService = registrationService;
    private readonly ILogger<RegistrationsController> _logger = logger;
    private readonly IValidator<UpdateTaskStatusRequestDto> _updateTaskStatusValidator = updateTaskStatusValidator;

    [HttpPatch("regulatorRegistrationTaskStatus/{id:int}")]
    [SwaggerOperation(
            Summary = "Updates regulator registration task status",
            Description = "Attempting to update regulator registration task status."
        )]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [SwaggerRequestExample(typeof(UpdateTaskStatusRequestDto), typeof(UpdateTaskStatusRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(BadRequestExample))]
    public async Task<IActionResult> UpdateRegulatorRegistrationTaskStatus([FromRoute] int id, [FromBody] UpdateTaskStatusRequestDto request)
    {
        await _updateTaskStatusValidator.ValidateAndThrowAsync(request);

        _logger.LogInformation("Attempting to update regulator registration task status");

        _ = await _registrationService.UpdateRegulatorRegistrationTaskStatus(id, request);

        return NoContent();
    }

    [HttpPatch("regulatorApplicationTaskStatus/{id:int}")]
    [SwaggerOperation(
            Summary = "Update regulator application task status",
            Description = "Attempting to update regulator application task status."
        )]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [SwaggerRequestExample(typeof(UpdateTaskStatusRequestDto), typeof(UpdateTaskStatusRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(BadRequestExample))]
    public async Task<IActionResult> UpdateRegulatorApplicationTaskStatus([FromRoute] int id, [FromBody] UpdateTaskStatusRequestDto request)
    {
        await _updateTaskStatusValidator.ValidateAndThrowAsync(request);

        _logger.LogInformation("Attempting to update regulator application task status");

        _ = await _registrationService.UpdateRegulatorApplicationTaskStatus(id, request);

        return NoContent();
    }
}
