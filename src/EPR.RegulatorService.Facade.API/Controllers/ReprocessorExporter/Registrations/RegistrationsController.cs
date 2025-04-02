using System;
using System.Net;
using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.API.Filters.Swashbuckle;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
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

    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [SwaggerRequestExample(typeof(UpdateTaskStatusRequestDto), typeof(UpdateTaskStatusRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(BadRequestExample))]
    [HttpPatch("regulatorRegistrationTaskStatus/{id:int}")]
    public async Task<IActionResult> UpdateRegulatorRegistrationTaskStatus([FromRoute] int id, [FromBody] UpdateTaskStatusRequestDto request)
    {
        try
        {
            ValidationResult validationResult = await _updateTaskStatusValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return HandleError.Handle(validationResult);
            }

            _logger.LogInformation($"Attempting to update regulator registration task status");

            _ = await _registrationService.UpdateRegulatorRegistrationTaskStatus(id, request);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update regulator registration task status");
            return HandleError.Handle(ex);
        }
    }


    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [SwaggerRequestExample(typeof(UpdateTaskStatusRequestDto), typeof(UpdateTaskStatusRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(BadRequestExample))]
    [HttpPatch("regulatorApplicationTaskStatus/{id:int}")]
    public async Task<IActionResult> UpdateRegulatorApplicationTaskStatus([FromRoute] int id, [FromBody] UpdateTaskStatusRequestDto request)
    {
        try
        {
            ValidationResult validationResult = await _updateTaskStatusValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return HandleError.Handle(validationResult);
            }

            _logger.LogInformation($"Attempting to update regulator application task status");

            _ = await _registrationService.UpdateRegulatorApplicationTaskStatus(id, request);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update regulator application task status");
            return HandleError.Handle(ex);
        }
    }
}
