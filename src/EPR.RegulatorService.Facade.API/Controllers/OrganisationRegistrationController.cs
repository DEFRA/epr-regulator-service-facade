using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.Registrations;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganisationRegistrationController : ControllerBase
    {
        private readonly ILogger<OrganisationRegistrationController> _logger;
        private readonly RegulatorUsers<OrganisationRegistrationController> _regulatorUsers;
        private readonly ICommonDataService _commonDataService;

        public OrganisationRegistrationController(
           ILogger<OrganisationRegistrationController> logger,
           IRegulatorOrganisationService regulatorOrganisationService,
           IOptions<OrganisationRegistrationController> messagingConfig,
           IMessagingService messagingService,
           ICommonDataService commonDataService)
        {
            _logger = logger;
            _regulatorUsers = new RegulatorUsers<OrganisationRegistrationController>(regulatorOrganisationService, _logger);
            _commonDataService = commonDataService;
        }


        [HttpGet]
        [Route("registrations/get-organisations")]
        public async Task<IActionResult> GetOrganisationRegistrations([FromQuery] OrganisationRegistrationFilter request)
        {
            // TODO: Test all the enum values for model validation correctness
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            // TODO: GetLastSyncTime from CommonDataService ?
            // TODO: GetLastRegistrationSyncTime from SubmissionService ?

            var registrationSubmissionsRequest = new GetOrganisationRegistrationRequest
            {
                UserId = User.UserId(),

                OrganisationName = request.OrganisationName,
                OrganisationReference = request.OrganisationReference,
                OrganisationType = request.OrganisationType.ToString(),
                Statuses = request.Statuses.ToString(),
                SubmissionYears = request.SubmissionYears,
                SubmissionPeriods = request.SubmissionPeriods,
                PageSize = request.PageSize,
                PageNumber = request.PageNumber
            };

            // TODO: Implement the real connection in _commonDataService
            // JsonOrganisationRegistrationHandler handles the load and filtering of the dummy data
            // CommonDataOrganisationRegistrationHandler will handle the request to Synapse when the endpoint is declared
            var registrations = await _commonDataService.GetOrganisationRegistrations<JsonOrganisationRegistrationHandler>(registrationSubmissionsRequest);
            if (!registrations.IsSuccessStatusCode)
            {
                return HandleError.HandleErrorWithStatusCode(registrations.StatusCode);
            }

            var paginatedResponse = await registrations.Content
                .ReadFromJsonAsync<PaginatedResponse<OrganisationRegistrationSummaryResponse>>();
            return Ok(paginatedResponse);
        }
    }
}
