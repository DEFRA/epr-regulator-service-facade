using Asp.Versioning;
using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}")]
[FeatureGate(FeatureFlags.ReprocessorExporter)]
public class RegistrationsController(IRegistrationService registrationService
    , IValidator<UpdateRegulatorRegistrationTaskDto> updateRegulatorRegistrationTaskValidator
    , IValidator<UpdateRegulatorApplicationTaskDto> updateRegulatorApplicationTaskValidator
    , IValidator<UpdateMaterialOutcomeRequestDto> updateMaterialOutcomeValidator
    , ILogger<RegistrationsController> logger) : ControllerBase
{

    [HttpPost("regulatorRegistrationTaskStatus")]
    [SwaggerOperation(
            Summary = "Updates a registration-level task (no associated material).",
            Description = "Attempting to update regulator registration task status."
        )]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [SwaggerResponse(StatusCodes.Status204NoContent, $"Returns No Content", typeof(NoContentResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "If the request is invalid or a validation error occurs.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> UpdateRegulatorRegistrationTaskStatus([FromBody] UpdateRegulatorRegistrationTaskDto request)
    {
        await updateRegulatorRegistrationTaskValidator.ValidateAndThrowAsync(request);

        logger.LogInformation(LogMessages.UpdateRegulatorRegistrationTaskStatus);

        _ = await registrationService.UpdateRegulatorRegistrationTaskStatus(request);

        return NoContent();
    }

    [HttpPost("regulatorApplicationTaskStatus")]
    [SwaggerOperation(
            Summary = "Updates a material-specific task status.",
            Description = "Attempting to update regulator application task status."
        )]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [SwaggerResponse(StatusCodes.Status204NoContent, $"Returns No Content", typeof(NoContentResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "If the request is invalid or a validation error occurs.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> UpdateRegulatorApplicationTaskStatus([FromBody] UpdateRegulatorApplicationTaskDto request)
    {
        await updateRegulatorApplicationTaskValidator.ValidateAndThrowAsync(request);

        logger.LogInformation(LogMessages.UpdateRegulatorApplicationTaskStatus);

        _ = await registrationService.UpdateRegulatorApplicationTaskStatus(request);

        return NoContent();
    }

    [HttpGet("registrations/{id:int}")]
    [ProducesResponseType(typeof(RegistrationOverviewDto), 200)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
            Summary = "get registration with materials and tasks",
            Description = "attempting to get registration with materials and tasks.  "
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns registration with materials and tasks.", typeof(RegistrationOverviewDto))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> GetRegistrationByRegistrationId(int id)
    {
        logger.LogInformation(LogMessages.RegistrationMaterialsTasks);
        var result = await registrationService.GetRegistrationByRegistrationId(id);
        return Ok(result);
    }

    [HttpGet("registrationMaterials/{id:int}")]
    [ProducesResponseType(typeof(RegistrationMaterialDetailsDto), 200)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
            Summary = "get summary info for a material",
            Description = "attempting to get summary info for a material.  "
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns summary info for a material.", typeof(RegistrationMaterialDetailsDto))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> GetRegistrationMaterialByRegistrationMaterialId(int id)
    {
        logger.LogInformation(LogMessages.SummaryInfoMaterial);
        var result = await registrationService.GetRegistrationMaterialByRegistrationMaterialId(id);
        return Ok(result);
    }

    [HttpPost("registrationMaterials/{id:int}/outcome")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(NoContentResult))]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(
            Summary = "update the outcome of a material registration",
            Description = "attempting to update the outcome of a material registration.  "
        )]
    [SwaggerResponse(StatusCodes.Status204NoContent, $"Returns No Content", typeof(NoContentResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "If the request is invalid or a validation error occurs.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> UpdateMaterialOutcomeByRegistrationMaterialId(
        [FromRoute] int id,
        [FromBody] UpdateMaterialOutcomeRequestDto request)
    {
        await updateMaterialOutcomeValidator.ValidateAndThrowAsync(request);
        logger.LogInformation(LogMessages.OutcomeMaterialRegistration);
        await registrationService.UpdateMaterialOutcomeByRegistrationMaterialId(id, request);
        return NoContent();
    }

    [HttpGet("registrationMaterials/{id:int}/wasteLicences")]
    [ProducesResponseType(typeof(RegistrationMaterialWasteLicenceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Show waste permit and exemption details for a material",
        Description = "Retrieve waste permit and exemption details for a specific material."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns waste permit and exemption details.", typeof(RegistrationMaterialWasteLicenceDto))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> GetWasteLicenceByRegistrationMaterialId(int id)
    {
        logger.LogInformation(LogMessages.WasteLicencesRegistrationMaterial, id);
        var result = await registrationService.GetWasteLicenceByRegistrationMaterialId(id);
        return Ok(result);
    }

    [HttpGet("registrationMaterials/{id:int}/reprocessingIO")]
    [ProducesResponseType(typeof(RegistrationMaterialReprocessingIODetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Show reprocessing inputs, outputs, and process description",
        Description = "Retrieve reprocessing inputs, outputs, and process description for a specific material."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns reprocessing inputs, outputs, and process details.", typeof(RegistrationMaterialReprocessingIODetailsDto))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> GetReprocessingInputsOutputsByRegistrationMaterialId(int id)
    {
        logger.LogInformation(LogMessages.ReprocessingInputsOutputsRegistrationMaterial, id);
        var result = await registrationService.GetReprocessingInputsOutputsByRegistrationMaterialId(id);
        return Ok(result);
    }

    [HttpGet("registrationMaterials/{id:int}/samplingPlan")]
    [ProducesResponseType(typeof(RegistrationMaterialSamplingPlanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Get sampling plan for a material",
        Description = "Retrieve sampling plan associated with a material."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns sampling plan for a material.", typeof(RegistrationMaterialSamplingPlanDto))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> GetSamplingPlanByRegistrationMaterialId(int id)
    {
        logger.LogInformation(LogMessages.SamplingPlanRegistrationMaterial, id);
        var result = await registrationService.GetSamplingPlanByRegistrationMaterialId(id);
        return Ok(result);
    }
}