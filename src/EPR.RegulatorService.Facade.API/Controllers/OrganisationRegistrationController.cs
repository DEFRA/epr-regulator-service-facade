using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.Registrations;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace EPR.RegulatorService.Facade.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganisationRegistrationController : ControllerBase
    {
        private readonly ILogger<OrganisationRegistrationController> _logger;
        private readonly ICommonDataService _commonDataService;

        public OrganisationRegistrationController(
           ILogger<OrganisationRegistrationController> logger,
           IOptions<MessagingConfig> messagingConfig,
           IMessagingService messagingService,
           ICommonDataService commonDataService)
        {
            _logger = logger;
            _commonDataService = commonDataService;
        }



        /// <summary>
        /// To do: Implement the real connection in _commonDataService
        /// JsonOrganisationRegistrationHandler handles the load and filtering of the dummy data
        /// CommonDataOrganisationRegistrationHandler will handle the request to Synapse when the endpoint is declared
        /// </summary>
        /// <param name="request">Filter parameters</param>
        /// <returns>BadRequest or OK</returns>
        [HttpGet]
        [Route("registrations/get-organisations")]
        public async Task<IActionResult> GetOrganisationRegistrations([FromQuery, Required] OrganisationRegistrationFilter request)
        {
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

            if (!ModelState.IsValid)
            {
                var validationProblem = new ValidationProblemDetails(ModelState)
                {
                    Title = "Validation Error",
                    Status = 400, // Ensure the status is explicitly set
                    Detail = "One or more validation errors occurred.",
                };
                return BadRequest(validationProblem);
            }

            var registrationSubmissionsRequest = new GetOrganisationRegistrationRequest
            {
                UserId = User.UserId(),

                OrganisationName = request.OrganisationName,
                OrganisationReference = request.OrganisationReference,
                OrganisationType = request.OrganisationType.ToString(),
                Statuses = request.Statuses.ToString(),
                RegistrationYears = request.RegistrationYear,
                PageSize = request.PageSize,
                PageNumber = request.PageNumber
            };

            var registrations = await _commonDataService.GetOrganisationRegistrations<JsonOrganisationRegistrationHandler>(registrationSubmissionsRequest);
            if (!registrations.IsSuccessStatusCode)
            {
                _logger.LogWarning("Didn't fetch Dummy data successfully");
                return HandleError.HandleErrorWithStatusCode(registrations.StatusCode);
            }

            var stringContent = await registrations.Content.ReadAsStringAsync();
            var paginatedResponse = JsonSerializer.Deserialize<PaginatedResponse<OrganisationRegistrationSummaryResponse>>(stringContent,
                     options);
            return Ok(paginatedResponse);
        }
    }
}
