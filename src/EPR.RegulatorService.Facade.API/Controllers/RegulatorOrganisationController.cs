using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using Microsoft.AspNetCore.Mvc;

namespace EPR.RegulatorService.Facade.API.Controllers
{
    [ApiController]
    [Route("api/regulator-organisation")]
    public class RegulatorOrganisationController : ControllerBase
    {
        private readonly IRegulatorOrganisationService _regulatorOrganisationService;
        private readonly ILogger<RegulatorOrganisationController> _logger;

        public RegulatorOrganisationController(IRegulatorOrganisationService regulatorOrganisationService, ILogger<RegulatorOrganisationController> logger)
        {
            _regulatorOrganisationService = regulatorOrganisationService;
            _logger = logger;
        }

        [HttpGet(Name = "GetRegulatorAccountByNation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetRegulatorAccountByNation([FromQuery] string nation)
        {
            var userId = User.UserId();

            if (userId == default)
            {
                _logger.LogError("UserId not available");
                return Problem("UserId not available", statusCode: StatusCodes.Status500InternalServerError);
            }

            var response = await _regulatorOrganisationService.GetRegulatorOrganisationByNation(nation);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRegulatorOrganisation([FromBody] CreateRegulatorAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            var userId = User.UserId();

            if (userId == default)
            {
                _logger.LogError("UserId not available");
                return Problem("UserId not available", statusCode: StatusCodes.Status500InternalServerError);
            }

            _logger.LogInformation($"Creating the selected regulator organisation {request.Name}");

            var response = await _regulatorOrganisationService.CreateRegulatorOrganisation(request);

            if (response.IsSuccess)
            {
                return CreatedAtRoute(nameof(GetRegulatorAccountByNation), new { nation = response.Value.Nation }, new
                {
                    response.Value.CreatedOn,
                    response.Value.ExternalId
                });
            }

            return HandleError.HandleErrorWithStatusCode(response.StatusCode);
        }
    }
}
