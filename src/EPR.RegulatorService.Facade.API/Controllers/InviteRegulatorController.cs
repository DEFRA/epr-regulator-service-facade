using Azure.Core;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Helpers;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using EPR.RegulatorService.Facade.Core.Services.ServiceRoles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;


namespace EPR.RegulatorService.Facade.API.Controllers
{
    [ApiController]
    [Route("api/accounts-management")]
    public class InviteRegulatorController : ControllerBase
    {
        private readonly ILogger<InviteRegulatorController> _logger;
        private readonly IRegulatorOrganisationService _regulatorOrganisationService;
        private readonly IServiceRolesLookupService _serviceRolesLookupService;

        public InviteRegulatorController(ILogger<InviteRegulatorController> logger,
            IRegulatorOrganisationService regulatorOrganisationService,
            IServiceRolesLookupService serviceRolesLookupService)
        {
            _logger = logger;
            _regulatorOrganisationService = regulatorOrganisationService;
            _serviceRolesLookupService = serviceRolesLookupService;
        }

        [HttpPost]
        [Route("invite-regulator-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateInviteEnrollment([FromBody] RegulatorInviteEnrollmentRequest request)
        {
            try
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

                var serviceRolesLookupModel = _serviceRolesLookupService.GetServiceRoles().SingleOrDefault(x => x.Key == request.RoleKey);

                if (serviceRolesLookupModel is null)
                {
                    _logger.LogError("Invalid role key: {key}", request.RoleKey);
                    return Problem("Invalid role key", statusCode: StatusCodes.Status500InternalServerError);
                }

                var mapping = new AddInviteUserRequest
                {
                    InvitedUser = new InvitedUser
                    {
                        Email = request.Email,
                        OrganisationId = request.OrganisationId,
                        PersonRoleId = serviceRolesLookupModel.PersonRoleId,
                        ServiceRoleId = serviceRolesLookupModel.ServiceRoleId,
                        UserId = request.UserId

                    },
                    InvitingUser = new InvitingUser
                    {
                        Email = request.Email,
                        UserId = userId
                    }
                };

                var regulatorInviteResponse = await _regulatorOrganisationService.RegulatorInvites(mapping);

                if (regulatorInviteResponse.IsSuccessStatusCode)
                {
                    return Ok(new
                    {
                        InvitedToken = await regulatorInviteResponse.Content.ReadAsStringAsync()
                    });
                }

                _logger.LogError("Failed to create invite regulator user");

                return HandleError.HandleErrorWithStatusCode(regulatorInviteResponse.StatusCode);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when creating the invite regulator user for id {0}", request.UserId);

                return HandleError.Handle(e);
            }
        }

        [HttpPost]
        [Route("enrol-invited-user")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EnrolInvitedUser([FromBody] EnrolInvitedUserRequest request)
        {
            try
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

                var response = await _regulatorOrganisationService.RegulatorEnrollment(request);

                if (response.IsSuccessStatusCode)
                {
                    return NoContent();
                }

                _logger.LogError("Failed to create enrollment");

                return HandleError.HandleErrorWithStatusCode(response.StatusCode);
            }
            catch (Exception e)
            {
                string logData = string.Format("Error when creating the enrollment for id {0}", request.UserId).Replace('\n', '_').Replace('\r', '_');
                _logger.LogError(logData, LogLevel.Error);

                return HandleError.Handle(e);
            }
        }

        [HttpGet]
        [Route("invited-regulator-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetInvitedEnrollment([FromQuery] string email)
        {
            try
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

                var regulatorInviteResponse = await _regulatorOrganisationService.RegulatorInvited(userId, email);

                if (regulatorInviteResponse.IsSuccessStatusCode)
                {
                    return Ok(new
                    {
                        InvitedToken = await regulatorInviteResponse.Content.ReadAsStringAsync()
                    });
                }

                _logger.LogError("Failed to retrive invite regulator user token");

                return HandleError.HandleErrorWithStatusCode(regulatorInviteResponse.StatusCode);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when retriving the invite regulator token for user {0}", User.UserId());

                return HandleError.Handle(e);
            }
        }
    }
}
