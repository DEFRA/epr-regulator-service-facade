using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;
using EPR.RegulatorService.Facade.Core.Models.Organisations;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Submissions;
using EPR.RegulatorService.Facade.Core.Models.Submissions.Events;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.API.Controllers;

[Route("api")]
public class SubmissionsController : ControllerBase
{
    private readonly ILogger<SubmissionsController> _logger;
    private readonly ISubmissionService _submissionService;
    private readonly MessagingConfig _messagingConfig;
    private readonly IMessagingService _messagingService;
    private readonly RegulatorUsers<SubmissionsController> _regulatorUsers;
    private readonly ICommonDataService _commonDataService;

    public SubmissionsController(
        ILogger<SubmissionsController> logger, 
        ISubmissionService submissionService, 
        IRegulatorOrganisationService regulatorOrganisationService,
        IOptions<MessagingConfig> messagingConfig, 
        IMessagingService messagingService,
        ICommonDataService commonDataService)
    {
        _logger = logger;
        _submissionService = submissionService;
        _messagingConfig = messagingConfig.Value;
        _messagingService = messagingService;
        _regulatorUsers = new RegulatorUsers<SubmissionsController>(regulatorOrganisationService, _logger);
        _commonDataService = commonDataService;
    }

    [HttpPost]
    [Route("pom/regulator-decision")]
    public async Task<IActionResult> RegulatorPoMDecisionEvent([FromBody] RegulatorPoMDecisionCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        var regulatorPoMDecisionEvent = new RegulatorPoMDecisionEvent
        {
            IsResubmissionRequired = request.IsResubmissionRequired,
            Decision = request.Decision,
            Comments = request.Comments,
            FileId = request.FileId
        };
        
        var submissionEvent = await _submissionService.CreateSubmissionEvent(request.SubmissionId, regulatorPoMDecisionEvent, User.UserId());
        
        if (submissionEvent.IsSuccessStatusCode)
        {
            var users = await _regulatorUsers.GetRegulatorUsers(User.UserId(), request.OrganisationId);
            var model = CreateBaseEmailModel(users, request);

            List<string> emailIds;
            if (request.Decision == RegulatorDecision.Accepted)
            {
                emailIds = _messagingService.SubmissionAccepted(model, EventType.RegulatorPoMDecision);
            }
            else
            {
                emailIds = request.IsResubmissionRequired 
                    ? _messagingService.SubmissionRejected(model, true) 
                    : _messagingService.SubmissionRejected(model, false);
            }
            
            return Ok(emailIds);
        }

        _logger.LogError("BadRequest");
        return new BadRequestResult();
    }

    [HttpGet]
    [Route("pom/get-submissions")]
    public async Task<IActionResult> GetPoMSubmissions([FromQuery] PoMSubmissionsFilters request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        var lastSyncResponse = await _commonDataService.GetSubmissionLastSyncTime();
        if (!lastSyncResponse.IsSuccessStatusCode)
        {
            return HandleError.HandleErrorWithStatusCode(lastSyncResponse.StatusCode);
        }

        var submissionEventsLastSync = lastSyncResponse.Content.ReadFromJsonAsync<SubmissionEventsLastSync>().Result;

        var deltaPoMDecisionsResponse = await _submissionService.GetDeltaPoMSubmissions(submissionEventsLastSync.LastSyncTime, User.UserId());
        
        if (!deltaPoMDecisionsResponse.IsSuccessStatusCode)
        {
            return HandleError.HandleErrorWithStatusCode(deltaPoMDecisionsResponse.StatusCode);
        }
        
        var deltaPoMDecisions = deltaPoMDecisionsResponse.Content.ReadFromJsonAsync<RegulatorPomDecision[]>().Result;

        var pomSubmissionsRequest = new GetPomSubmissionsRequest
        {
            OrganisationName = request.OrganisationName,
            OrganisationReference = request.OrganisationReference,
            OrganisationType = request.OrganisationType,
            PageNumber = request.PageNumber,
            Statuses = request.Statuses,
            SubmissionYears = request.SubmissionYears,
            SubmissionPeriods = request.SubmissionPeriods,
            UserId = User.UserId(),
            DecisionsDelta = deltaPoMDecisions,
            PageSize = request.PageSize
        };

        var submissions = await _commonDataService.GetPoMSubmissions(pomSubmissionsRequest);
        if (submissions.IsSuccessStatusCode)
        {
            var paginatedResponse = await submissions.Content
                .ReadFromJsonAsync<PaginatedResponse<PomSubmissionSummaryResponse>>();
            return Ok(paginatedResponse);
        }

        return HandleError.HandleErrorWithStatusCode(submissions.StatusCode);
    }
    
    [HttpPost]
    [Route("registration/regulator-decision")]
    public async Task<IActionResult> CreateRegulatorRegistrationDecisionEvent([FromBody] RegulatorRegistrationDecisionCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        var regulatorRegistrationDecisionEvent = new RegulatorRegistrationDecisionEvent
        {
            Decision = request.Decision,
            Comments = request.Comments,
            FileId = request.FileId
        };
        
        var submissionEvent = await _submissionService.CreateSubmissionEvent(request.SubmissionId, regulatorRegistrationDecisionEvent, User.UserId());
        
        if (submissionEvent.IsSuccessStatusCode)
        {
            var users = await _regulatorUsers.GetRegulatorUsers(User.UserId(), request.OrganisationId);
            var model = CreateBaseEmailModel(users, request);
           
            List<string> emailIds;
            if (request.Decision == RegulatorDecision.Accepted)
            {
                emailIds = _messagingService.SubmissionAccepted(model, EventType.RegulatorRegistrationDecision);
            }
            else
            {
                emailIds = _messagingService.SubmissionRejected(model, null);
            }
            
            return Ok(emailIds);
        }

        return new BadRequestResult();
    }
    
    [HttpGet]
    [Route("registrations/get-submissions")]
    public async Task<IActionResult> GetRegistrationSubmissions([FromQuery] RegistrationSubmissionsFilters request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var lastSyncResponse = await _commonDataService.GetSubmissionLastSyncTime();
        if (!lastSyncResponse.IsSuccessStatusCode)
        {
            return HandleError.HandleErrorWithStatusCode(lastSyncResponse.StatusCode);
        }

        var submissionEventsLastSync = lastSyncResponse.Content.ReadFromJsonAsync<SubmissionEventsLastSync>().Result;

        var deltaRegistrationDecisionsResponse = await _submissionService.GetDeltaRegistrationSubmissions(submissionEventsLastSync.LastSyncTime, User.UserId());
        
        if (!deltaRegistrationDecisionsResponse.IsSuccessStatusCode)
        {
            return HandleError.HandleErrorWithStatusCode(deltaRegistrationDecisionsResponse.StatusCode);
        }
        
        var deltaRegistrationDecisions = deltaRegistrationDecisionsResponse.Content.ReadFromJsonAsync<RegulatorRegistrationDecision[]>().Result;

        var registrationSubmissionsRequest = new GetRegistrationSubmissionsRequest
        {
            OrganisationName = request.OrganisationName,
            OrganisationReference = request.OrganisationReference,
            OrganisationType = request.OrganisationType,
            PageNumber = request.PageNumber,
            Statuses = request.Statuses,
            SubmissionYears = request.SubmissionYears,
            SubmissionPeriods = request.SubmissionPeriods,
            UserId = User.UserId(),
            DecisionsDelta = deltaRegistrationDecisions,
            PageSize = request.PageSize
        };

        var submissions = await _commonDataService.GetRegistrationSubmissions(registrationSubmissionsRequest);
        if (submissions.IsSuccessStatusCode)
        {
            var paginatedResponse = await submissions.Content
                .ReadFromJsonAsync<PaginatedResponse<RegistrationSubmissionSummaryResponse>>();
            return Ok(paginatedResponse);
        }

        return HandleError.HandleErrorWithStatusCode(submissions.StatusCode);
    }
    
    private SubmissionEmailModel CreateBaseEmailModel(List<OrganisationUser> users, AbstractDecisionRequest request)
    {
        var userEmails = users.Select(user => new UserEmailModel
        {
            Email = user.Email, 
            FirstName = user.FirstName, 
            LastName = user.LastName
        }).ToList();

        var model = new SubmissionEmailModel()
        {
            UserEmails = userEmails,
            OrganisationNumber = request.OrganisationNumber,
            OrganisationName = request.OrganisationName,
            SubmissionPeriod = request.SubmissionPeriod,
            RejectionComments = request.Comments,
            AccountLoginUrl = _messagingConfig.AccountSignInUrl
        };

        return model;
    }
}