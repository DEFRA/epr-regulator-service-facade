using Microsoft.AspNetCore.Mvc;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Accounts;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using Microsoft.Extensions.Options;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;

namespace EPR.RegulatorService.Facade.API.Controllers;

public class ApplicationController : ControllerBase
{
    private readonly ILogger<ApplicationController> _logger;
    private readonly MessagingConfig _messagingConfig;
    private readonly IMessagingService _messagingService;

    public ApplicationController(ILogger<ApplicationController> logger, IOptions<MessagingConfig> messagingConfig, IMessagingService messagingService)
    {
        _logger = logger;
        _messagingConfig = messagingConfig.Value;
        _messagingService = messagingService;
    }
    
    [HttpPost]
    [Route("api/regulators/accounts/govNotification")]
    public async Task<IActionResult> GovNotification([FromBody] GovNotificationRequestModel request)
    {
        var model = CreateBaseEmailModel(request);
        
        if (request.Decision == ApplicationDecision.Approved)
        {
            if (request.DelegatedUsers.Count == 0)
            {
                _logger.LogInformation($"Attempting to send ApprovedPersonAccepted email");
                string? messageId = _messagingService.ApprovedPersonAccepted(model);
                _logger.LogInformation($"Email sent: {messageId}");
                return Ok(messageId);
            }
            else if (request.DelegatedUsers.Count == 1)
            {
                _logger.LogInformation($"Attempting to send DelegatedPersonAccepted emails");
                var delegatedUser = request.DelegatedUsers.Single();
                model.DelegatedPeople.Add(new UserEmailModel()
                {
                    Email = delegatedUser.Email,
                    FirstName = delegatedUser.UserFirstName,
                    LastName = delegatedUser.UserSurname
                });

                var messageIds = _messagingService.DelegatedPersonAccepted(model);
                _logger.LogInformation($"Emails sent:{String.Join(',', messageIds)}");
                return Ok(messageIds);
            }
        }
        else if (request.Decision == ApplicationDecision.Rejected)
        {
            if (request.RegulatorRole == RegulatorRole.Approved)
            {
                _logger.LogInformation($"Attempting to send ApprovedPersonRejected emails");
                model.RejectionComments = request.RejectionComments;
                CopyDelegatedUsersFromRequestToModel(request, model);
                
                var messageIds = _messagingService.ApprovedPersonRejected(model);
                _logger.LogInformation($"Emails sent:{String.Join(',', messageIds)}");
                return Ok(messageIds);
            }
            else if (request.RegulatorRole == RegulatorRole.Delegated)
            {
                _logger.LogInformation($"Attempting to send DelegatedPersonRejected emails");
                model.RejectionComments = request.RejectionComments;
                CopyDelegatedUsersFromRequestToModel(request, model);
                
                var messageIds = _messagingService.DelegatedPersonRejected(model);
                _logger.LogInformation($"Emails sent:{String.Join(',', messageIds)}");
                return Ok(messageIds);
            }
        }

        throw new Exception("Invalid Request");
    }

    private void CopyDelegatedUsersFromRequestToModel(GovNotificationRequestModel request, ApplicationEmailModel model)
    {
        request.DelegatedUsers.ForEach(delegatedPerson => 
            model.DelegatedPeople.Add(new UserEmailModel()
            {
                Email = delegatedPerson.Email,
                FirstName = delegatedPerson.UserFirstName,
                LastName = delegatedPerson.UserSurname
            })
        );
    }
    
    private ApplicationEmailModel CreateBaseEmailModel(GovNotificationRequestModel request)
    {
        var model = new ApplicationEmailModel()
        {
            ApprovedPerson = new UserEmailModel()
            {
                Email = request.ApprovedUser.Email,
                FirstName = request.ApprovedUser.UserFirstName,
                LastName = request.ApprovedUser.UserSurname
            },
            OrganisationNumber = request.OrganisationNumber,
            OrganisationName = request.OrganisationName,
            AccountLoginUrl = _messagingConfig.AccountSignInUrl
        };

        return model;
    }
}
