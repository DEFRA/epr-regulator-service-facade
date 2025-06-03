using Asp.Versioning;
using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Extensions;
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
public class RegistrationMaterialsController(IReprocessorExporterService reprocessorExporterService
    , IValidator<UpdateMaterialOutcomeRequestDto> updateMaterialOutcomeValidator
    , IValidator<OfflinePaymentRequestDto> offlinePaymentRequestDtoValidator
    , IValidator<MarkAsDulyMadeRequestDto> markAsDulyMadeRequestDtoValidator
    , ILogger<RegistrationMaterialsController> logger) : ControllerBase
{   
    [HttpGet("registrationMaterials/{id}")]
    [ProducesResponseType(typeof(RegistrationMaterialDetailsDto), 200)]
    [SwaggerOperation(
            Summary = "get summary info for a material",
            Description = "attempting to get summary info for a material.  "
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns summary info for a material.", typeof(RegistrationMaterialDetailsDto))]
    public async Task<IActionResult> GetRegistrationMaterialByRegistrationMaterialId(Guid id)
    {
        logger.LogInformation(LogMessages.SummaryInfoMaterial);
        var result = await reprocessorExporterService.GetRegistrationMaterialByRegistrationMaterialId(id);
        return Ok(result);
    }

    [HttpPost("registrationMaterials/{id}/outcome")]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(
            Summary = "update the outcome of a material registration",
            Description = "attempting to update the outcome of a material registration.  "
        )]
    public async Task<IActionResult> UpdateMaterialOutcomeByRegistrationMaterialId(
        [FromRoute] Guid id,
        [FromBody] UpdateMaterialOutcomeRequestDto request)
    {
        await updateMaterialOutcomeValidator.ValidateAndThrowAsync(request);
        logger.LogInformation(LogMessages.OutcomeMaterialRegistration);
        await reprocessorExporterService.UpdateMaterialOutcomeByRegistrationMaterialId(id, request);
        return NoContent();
    }

    [HttpGet("registrationMaterials/{id}/wasteLicences")]
    [ProducesResponseType(typeof(RegistrationMaterialWasteLicencesDto), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Show waste permit and exemption details for a material",
        Description = "Retrieve waste permit and exemption details for a specific material."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns waste permit and exemption details.", typeof(RegistrationMaterialWasteLicencesDto))]
    public async Task<IActionResult> GetWasteLicenceByRegistrationMaterialId(Guid id)
    {
        logger.LogInformation(LogMessages.WasteLicencesRegistrationMaterial, id);
        var result = await reprocessorExporterService.GetWasteLicenceByRegistrationMaterialId(id);
        return Ok(result);
    }

    [HttpGet("registrationMaterials/{id}/reprocessingIO")]
    [ProducesResponseType(typeof(RegistrationMaterialReprocessingIODto), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Show reprocessing inputs, outputs, and process description",
        Description = "Retrieve reprocessing inputs, outputs, and process description for a specific material."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns reprocessing inputs, outputs, and process details.", typeof(RegistrationMaterialReprocessingIODto))]
    public async Task<IActionResult> GetReprocessingIOByRegistrationMaterialId(Guid id)
    {
        logger.LogInformation(LogMessages.ReprocessingIORegistrationMaterial, id);
        var result = await reprocessorExporterService.GetReprocessingIOByRegistrationMaterialId(id);
        return Ok(result);
    }

    [HttpGet("registrationMaterials/{id}/samplingPlan")]
    [ProducesResponseType(typeof(RegistrationMaterialSamplingPlanDto), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get sampling plan for a material",
        Description = "Retrieve sampling plan associated with a material."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns sampling plan for a material.", typeof(RegistrationMaterialSamplingPlanDto))]
    public async Task<IActionResult> GetSamplingPlanByRegistrationMaterialId(Guid id)
    {
        logger.LogInformation(LogMessages.SamplingPlanRegistrationMaterial, id);
        var result = await reprocessorExporterService.GetSamplingPlanByRegistrationMaterialId(id);
        return Ok(result);
    }

    [HttpGet("registrationMaterials/{id}/paymentFees")]
    [ProducesResponseType(typeof(PaymentFeeDetailsDto), StatusCodes.Status200OK)]
    [SwaggerOperation(
    Summary = "Get registration fee details.",
    Description = "Attempting to get registration fee details."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns registration fee details.", typeof(PaymentFeeDetailsDto))]
    public async Task<IActionResult> GetPaymentFeeDetailsByRegistrationMaterialId(Guid id)
    {
        logger.LogInformation(LogMessages.AttemptingRegistrationFeeDetails);
        var result = await reprocessorExporterService.GetPaymentFeeDetailsByRegistrationMaterialId(id);
        return Ok(result);
    }

    [HttpPost("registrationMaterials/offlinePayment")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ContentResult))]
    [SwaggerOperation(
            Summary = "Saves a new offline payment",
            Description = "Save a new offline payment with mandatory payment request data.  "
        )]
    public async Task<IActionResult> SaveOfflinePayment([FromBody] OfflinePaymentRequestDto request)
    {
        await offlinePaymentRequestDtoValidator.ValidateAndThrowAsync(request);
        logger.LogInformation(LogMessages.SaveOfflinePayment);
        await reprocessorExporterService.SaveOfflinePayment(User.UserId(), request);
        return NoContent();
    }

    [HttpPost("registrationMaterials/{id}/markAsDulyMade")]
    [SwaggerOperation(
            Summary = "Mark a registration material as duly made”",
            Description = "Attempting to mark a registration material as duly made. "
        )]
    public async Task<IActionResult> MarkAsDulyMadeByRegistrationMaterialId(
        [FromRoute] Guid id, 
        [FromBody] MarkAsDulyMadeRequestDto request)
    {
        await markAsDulyMadeRequestDtoValidator.ValidateAndThrowAsync(request);
        logger.LogInformation(LogMessages.AttemptingMarkAsDulyMade);
        await reprocessorExporterService.MarkAsDulyMadeByRegistrationMaterialId(id, User.UserId(), request);
        return NoContent();
    }   
}