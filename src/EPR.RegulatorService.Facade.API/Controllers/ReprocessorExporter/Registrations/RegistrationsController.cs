using EPR.RegulatorService.Facade.Core.Models.PrnBackends;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations
{
    [ApiController]
    [Route("registrations")]
    [ExcludeFromCodeCoverage]
    public class RegistrationsController : ControllerBase//This controller needs removing when the test is completed. This controller is only used for testing
    {
        private readonly ILogger<RegistrationsController> _logger;
        private readonly IRegistrationsService _registrationsService;

        public RegistrationsController(ILogger<RegistrationsController> logger, IRegistrationsService registrationsService)
        {
            _logger = logger;
            _registrationsService = registrationsService;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(List<PrnBackendModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPrnByOrganisationId()
        {
            Guid orgId = new Guid("B0F6CBAC-8DFE-4C7C-950F-7D774EB14B20");
            _logger.LogInformation("RegistrationsController - GetAllPrnByOrganisationId: orgId {OrganisationId}", orgId);
            return new OkObjectResult(await _registrationsService.GetAllPrnByOrganisationId(orgId));
        }
    }
}
