using Asp.Versioning;
using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;
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
public class AccreditationsController(
    IReprocessorExporterService reprocessorExporterService,
    IValidator<MarkAsDulyMadeRequestDto> markAsDulyMadeRequestDtoValidator,
    IValidator<UpdateAccreditationTaskStatusDto> updateAccreditationMaterialTaskValidator,
    IValidator<OfflinePaymentRequestDto> offlinePaymentRequestDtoValidator,
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
    public async Task<IActionResult> GetRegistrationByIdWithAccreditationsAsync(Guid id, [FromQuery] int? year)
    {
        logger.LogInformation(LogMessages.RegistrationAccreditationTasks);
        var accreditations = await reprocessorExporterService.GetRegistrationByIdWithAccreditationsAsync(id, year);
        return Ok(accreditations);
    }

    [HttpGet("accreditationMaterials/{id:guid}/paymentFees")]
    [ProducesResponseType(typeof(AccreditationPaymentFeeDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Get accreditation fee details by registered material id.",
        Description = "Attempting to get accreditation fee details."
    )]
    
    [SwaggerResponse(StatusCodes.Status200OK, "Returns accreditation fee details.", typeof(AccreditationPaymentFeeDetailsDto))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> GetAccreditationPaymentFeeDetailsByAccreditationId(Guid id)
    {
        logger.LogInformation(LogMessages.AttemptingAccreditationFeeDetails);
        var result = await reprocessorExporterService.GetAccreditationPaymentFeeDetailsByAccreditationId(id);
        return Ok(result);
    }
    
    [HttpPost("accreditations/{id}/markAsDulyMade")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(NoContentResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ContentResult))]
    [SwaggerOperation(
        Summary = "Mark a accreditation material as duly made”",
        Description = "Attempting to mark a accreditation material as duly made. "
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, $"Returns No Content", typeof(NoContentResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "If the request is invalid or a validation error occurs.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> MarkAsDulyMadeByAccreditationId(
        [FromRoute] Guid id,
        [FromBody] MarkAsDulyMadeRequestDto request)
    {
        await markAsDulyMadeRequestDtoValidator.ValidateAndThrowAsync(request);
        logger.LogInformation(LogMessages.AttemptingMarkAccreditationMaterialAsDulyMade);
        await reprocessorExporterService.MarkAsDulyMadeByAccreditationId(id, User.UserId(), request);
        return NoContent();
    }

    [HttpPost("regulatorAccreditationTaskStatus")]
    [SwaggerOperation(
        Summary = "Updates a accreditation task status.",
        Description = "Attempting to update regulator accreditation task status."
    )]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [SwaggerResponse(StatusCodes.Status204NoContent, $"Returns No Content", typeof(NoContentResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "If the request is invalid or a validation error occurs.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> UpdateRegulatorAccreditationTaskStatus([FromBody] UpdateAccreditationTaskStatusDto request)
    {
        await updateAccreditationMaterialTaskValidator.ValidateAndThrowAsync(request);
        logger.LogInformation(LogMessages.UpdateRegulatorAccreditationTaskStatus, request.Status);
        await reprocessorExporterService.UpdateRegulatorAccreditationTaskStatus(User.UserId(), request);
        return NoContent();
    }


    [HttpPost("accreditations/offlinePayment")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(NoContentResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ContentResult))]
    [SwaggerOperation(
        Summary = "Saves a new offline payment",
        Description = "Save a new offline payment with mandatory payment request data.  "
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, $"Returns No Content", typeof(NoContentResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "If the request is invalid or a validation error occurs.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> SaveAccreditationOfflinePayment([FromBody] OfflinePaymentRequestDto request)
    {
        await offlinePaymentRequestDtoValidator.ValidateAndThrowAsync(request);
        logger.LogInformation(LogMessages.SaveAccreditationOfflinePayment);
        await reprocessorExporterService.SaveAccreditationOfflinePayment(User.UserId(), request);
        return NoContent();
    }
}