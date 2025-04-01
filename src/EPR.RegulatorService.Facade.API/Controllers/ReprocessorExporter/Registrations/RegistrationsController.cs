using System.Net;
using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.API;
using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.API.Filters.Swashbuckle;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;

[Route("api/[controller]")]
public class RegistrationsController : ControllerBase
{
    private readonly IRegistrationService _registrationService;
    private readonly ILogger<RegistrationsController> _logger;

    public RegistrationsController(IRegistrationService registrationService, ILogger<RegistrationsController> logger)
    {
        _registrationService = registrationService;
        _logger = logger;
    }

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
