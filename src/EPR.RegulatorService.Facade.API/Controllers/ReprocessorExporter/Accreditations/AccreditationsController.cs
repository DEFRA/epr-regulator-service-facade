using Asp.Versioning;
using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Accreditations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using EPR.RegulatorService.Facade.API.Extensions;

namespace EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Accreditations;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}")]
[FeatureGate(FeatureFlags.ReprocessorExporter)]
public class AccreditationsController(IAccreditationService accreditationService,
                                    IValidator<AccreditationMarkAsDulyMadeRequestDto> accreditationMarkAsDulyMadeRequestDtoValidator,
                                    IValidator<UpdateAccreditationMaterialTaskStatusDto> updateAccreditationMaterialTaskValidator,
                                    ILogger<AccreditationsController> logger
                                    ) : ControllerBase
{

    [HttpGet("accreditationMaterials/{id:guid}/paymentFees")]
    [ProducesResponseType(typeof(AccreditationPaymentFeeDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Get accreditation fee details.",
        Description = "Attempting to get accreditation fee details."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns accreditation fee details.", typeof(AccreditationPaymentFeeDetailsDto))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> GetPaymentFeeDetailsByAccreditationMaterialId(Guid id)
    {
        logger.LogInformation(LogMessages.AttemptingAccreditationFeeDetails);
        var result = await accreditationService.GetPaymentFeeDetailsByAccreditationMaterialId(id);
        return Ok(result);
    }


    [HttpPost("accreditationMaterials/markAsDulyMade")]
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
    public async Task<IActionResult> MarkAccreditationMaterialStatusAsDulyMade([FromBody] AccreditationMarkAsDulyMadeRequestDto request)
    {
        await accreditationMarkAsDulyMadeRequestDtoValidator.ValidateAndThrowAsync(request);
        logger.LogInformation(LogMessages.AttemptingMarkAccreditationMaterialAsDulyMade);
        await accreditationService.MarkAccreditationMaterialStatusAsDulyMade(User.UserId(), request);
        return NoContent();
    }

    [HttpPost("updateAccreditationMaterialTaskStatus")]
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
    public async Task<IActionResult> UpdateAccreditationMaterialTaskStatus([FromBody] UpdateAccreditationMaterialTaskStatusDto request)
    {
        await updateAccreditationMaterialTaskValidator.ValidateAndThrowAsync(request);
        logger.LogInformation(LogMessages.UpdateAccreditationMaterialTaskStatus, request.TaskStatus);
        await accreditationService.UpdateAccreditationMaterialTaskStatus(User.UserId(), request);
        return NoContent();
    }


}