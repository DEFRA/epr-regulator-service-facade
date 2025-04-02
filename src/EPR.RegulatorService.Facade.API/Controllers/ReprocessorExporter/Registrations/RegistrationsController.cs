using System;
using System.Net;
using Asp.Versioning;
using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Constants;
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

    [ProducesResponseType(typeof(RegistrationOverviewDto), 200)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("registrations/{id:int}")]
    public async Task<IActionResult> GetRegistrationByRegistrationId(int id)
    {
        try
        {
            logger.LogInformation(LogMessages.RegistrationMaterialsTasks);

            var result = await registrationService.GetRegistrationByRegistrationId(id);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get registration with materials and tasks");
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
            logger.LogInformation(LogMessages.SummaryInfoMaterial);

            var result = await registrationService.GetRegistrationMaterialByRegistrationMaterialId(id);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get summary info for a material");
            return HandleError.Handle(ex);
        }
    }

    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [HttpPatch("registrationMaterials/{id:int}/outcome")]
    public async Task<IActionResult> UpdateMaterialOutcomeByRegistrationMaterialId(
            [FromRoute] int id, 
            [FromBody] UpdateMaterialOutcomeRequestDto request)
    {
        try
        {
            ValidationResult validationResult = await updateMaterialOutcomeValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return HandleError.Handle(validationResult);
            }

            logger.LogInformation(LogMessages.OutcomeMaterialRegistration);

            _ = await registrationService.UpdateMaterialOutcomeByRegistrationMaterialId(id, request);

            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update the outcome of a material registration");
            return HandleError.Handle(ex);
        }
    }
}