using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using Microsoft.AspNetCore.Mvc;

namespace EPR.RegulatorService.Facade.API.Controllers
{
    [Route("api")]
    public class OrganisationRegistrationController : Controller
    {
        private readonly ILogger<OrganisationRegistrationController> _logger;
        private readonly ISubmissionService _submissionService;
        private readonly IMessagingService _messagingService;
        private readonly RegulatorUsers<OrganisationRegistrationController> _regulatorUsers;


        public OrganisationRegistrationController(
        ILogger<OrganisationRegistrationController> logger,
        ISubmissionService submissionService,
        IRegulatorOrganisationService regulatorOrganisationService,
        IMessagingService messagingService)
        {
            _logger = logger;
            _submissionService = submissionService;
            _messagingService = messagingService;
            _regulatorUsers = new RegulatorUsers<OrganisationRegistrationController>(regulatorOrganisationService, _logger);
        }

        [HttpPost]
        [Route("registration/registration-submission-decision")]
        public async Task<IActionResult> CreateRegistrationSubmissionDecisionEvent([FromBody] RegistrationSubmissionDecisionCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            var registrationSubmissionDecisionEvent = new RegistrationSubmissionDecisionEvent
            {
                OrganisationId = request.OrganisationId,
                SubmissionId = request.SubmissionId,
                RegistrationSubmissionDecision = request.Decision,
                RegulatorComment = request.RegulatorComment
            };

            var submissionEvent = await _submissionService.CreateRegistrationDecisionEvent(
                registrationSubmissionDecisionEvent,
                User.UserId());

            if (submissionEvent.IsSuccessStatusCode)
            {
                var users = await _regulatorUsers.GetRegulatorUsers(User.UserId(), request.OrganisationId);
                var model = CreateBaseEmailModel(users, request);

                List<string> emailIds;
                if (request.Decision == RegulatorDecision.Accepted)
                {
                    emailIds = _messagingService.SubmissionAccepted(model, EventType.RegulatorRegistrationDecision);
                }
                else if (request.Decision == RegulatorDecision.Rejected)
                {
                    emailIds = _messagingService.SubmissionRejected(model, null);
                }
                else if (request.Decision == RegulatorDecision.Cancelled)
                {
                    emailIds = _messagingService.SubmissionCancelled(model, null);
                }
                else
                {
                    return Problem("Unable to submit decision request");
                }

                return Ok(emailIds);
            }

            return new BadRequestResult();
        }
    }
}
