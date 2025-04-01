using System;
using System.Net;
using Asp.Versioning;
using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}")]
[FeatureGate(FeatureFlags.ReprocessorExporter)]
public class RegistrationsController(IRegistrationService registrationService
    , IValidator<UpdateMaterialOutcomeRequestDto> updateMaterialOutcomeValidator
    , ILogger<RegistrationsController> logger) : ControllerBase
{
    private readonly IRegistrationService _registrationService = registrationService;
    private readonly ILogger<RegistrationsController> _logger = logger; 
    private readonly IValidator<UpdateMaterialOutcomeRequestDto> _updateMaterialOutcomeValidator = updateMaterialOutcomeValidator;

    [ProducesResponseType(typeof(RegistrationOverviewDto), 200)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("registrations/{id:int}")]
    public async Task<IActionResult> GetRegistrationByRegistrationId(int id)
    {
        try
        {
            _logger.LogInformation($"Get registration with materials and tasks");

            var result = await _registrationService.GetRegistrationByRegistrationId(id);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get registration with materials and tasks");
            return HandleError.Handle(ex);
        }
    }

    [ProducesResponseType(typeof(RegistrationOverviewDto), 200)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("registrationMaterials/{id:int}")]
    public async Task<IActionResult> GetRegistrationMaterialByRegistrationMaterialId(int id)
    {
        try
        {
            _logger.LogInformation($"Get summary info for a material");

            var result = await _registrationService.GetRegistrationMaterialByRegistrationMaterialId(id);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get summary info for a material");
            return HandleError.Handle(ex);
        }
    }

    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [HttpPatch("registrationMaterials/{id:int}/outcome")]
    public async Task<IActionResult> UpdateMaterialOutcomeByRegistrationMaterialId([FromRoute] int id, [FromBody] UpdateMaterialOutcomeRequestDto request)
    {
        try
        {
            ValidationResult validationResult = await _updateMaterialOutcomeValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return HandleError.Handle(validationResult);
            }

            _logger.LogInformation($"Updates the outcome of a material registration");

            _ = await _registrationService.UpdateMaterialOutcomeByRegistrationMaterialId(id, request);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update the outcome of a material registration");
            return HandleError.Handle(ex);
        }
    }
}