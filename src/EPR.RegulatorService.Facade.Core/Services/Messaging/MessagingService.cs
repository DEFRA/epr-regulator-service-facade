﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Interfaces;
using System.Diagnostics;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Extensions;
using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;
using EPR.RegulatorService.Facade.Core.Models.Organisations;
using System.Text.RegularExpressions;

namespace EPR.RegulatorService.Facade.Core.Services.Messaging;

public class MessagingService : IMessagingService
{
    private readonly INotificationClient _notificationClient;
    private readonly MessagingConfig _messagingConfig;
    private readonly ILogger<MessagingService> _logger;
    private readonly EprPackagingRegulatorEmailConfig _eprPackagingRegulatorEmailConfig;
    private const int timeoutInSeconds = 60;
    private const string ExceptionLogMessage = "GOV UK NOTIFY ERROR. Method: SendEmail, Organisation ID: {OrganisationId}, User ID: {UserId}, Template: {TemplateId}";

    public MessagingService(INotificationClient notificationClient, 
        IOptions<MessagingConfig> messagingConfig,
        IOptions<EprPackagingRegulatorEmailConfig> eprPackagingRegulatorEmailConfig,
        ILogger<MessagingService> logger)
    {
        _notificationClient = notificationClient;
        _messagingConfig = messagingConfig.Value;
        _eprPackagingRegulatorEmailConfig = eprPackagingRegulatorEmailConfig.Value;
        _logger = logger;
    }

    public string? ApprovedPersonAccepted(ApplicationEmailModel model)
    {
        ValidateRequiredApplicationEmailModelParameters(model);

        var approvedPersonParameters = new Dictionary<string, object>
        {
            { "firstName", model.ApprovedPerson.FirstName },
            { "lastName", model.ApprovedPerson.LastName },
            { "organisationNumber", model.OrganisationNumber.ToReferenceNumberFormat() },
            { "organisationName", model.OrganisationName },
            { "accountLoginUrl", model.AccountLoginUrl }
        };

        return SendEmail(model.ApprovedPerson.Email, _messagingConfig.ToApprovedPersonApprovedPersonAccepted, approvedPersonParameters);
    }

    public List<string> ApprovedPersonRejected(ApplicationEmailModel model)
    {
        ValidateRequiredApplicationEmailModelParameters(model);
        ValidateStringParameter(model.RejectionComments, nameof(model.RejectionComments));

        foreach (var delegatedPerson in model.DelegatedPeople)
        {
            ValidateUserEmailParameters(delegatedPerson);
        }

        var emailIds = new List<string>();

        var approvedPersonParameters = new Dictionary<string, object>
        {
            { "firstName", model.ApprovedPerson.FirstName },
            { "lastName", model.ApprovedPerson.LastName },
            { "organisationNumber", model.OrganisationNumber.ToReferenceNumberFormat() },
            { "organisationName", model.OrganisationName },
            { "reasonForRejection", model.RejectionComments },
            { "accountLoginUrl", model.AccountLoginUrl }
        };

        var emailId = SendEmail(model.ApprovedPerson.Email, _messagingConfig.ToApprovedPersonApprovedPersonRejected, approvedPersonParameters);
        emailIds.Add(emailId);

        foreach (var delegatedPerson in model.DelegatedPeople)
        {
            var delegatedPersonParameters = new Dictionary<string, object>
            {
                { "firstName", delegatedPerson.FirstName },
                { "lastName", delegatedPerson.LastName },
                { "organisationNumber", model.OrganisationNumber.ToReferenceNumberFormat() },
                { "organisationName", model.OrganisationName },
                { "approvedPersonFirstName", model.ApprovedPerson.FirstName },
                { "approvedPersonLastName", model.ApprovedPerson.LastName },
            };

            emailId = SendEmail(delegatedPerson.Email, _messagingConfig.ToDelegatedPersonApprovedPersonRejected, delegatedPersonParameters);
            emailIds.Add(emailId);
        }
        
        return emailIds;
    }

    public List<string> DelegatedPersonAccepted(ApplicationEmailModel model)
    {
        ValidateRequiredApplicationEmailModelParameters(model);

        var delegatedPerson = model.DelegatedPeople.Single();
        ValidateUserEmailParameters(delegatedPerson);

        var emailIds = new List<string>();

        var approvedPersonParameters = new Dictionary<string, object>
        {
            { "delegatedPerson", $"{delegatedPerson.FirstName} {delegatedPerson.LastName}" },
            { "firstName", model.ApprovedPerson.FirstName },
            { "lastName", model.ApprovedPerson.LastName },
            { "organisationNumber", model.OrganisationNumber.ToReferenceNumberFormat() },
            { "organisationName", model.OrganisationName },
            { "reasonForRejection", model.RejectionComments },
            { "accountLoginUrl", model.AccountLoginUrl }
        };

        var emailId = SendEmail(model.ApprovedPerson.Email, _messagingConfig.ToApprovedPersonDelegatedPersonAccepted, approvedPersonParameters);
        emailIds.Add(emailId);

        var delegatedPersonParameters = new Dictionary<string, object>
        {
            { "firstName", delegatedPerson.FirstName },
            { "lastName", delegatedPerson.LastName },
            { "organisationNumber", model.OrganisationNumber.ToReferenceNumberFormat() },
            { "organisationName", model.OrganisationName },
            { "accountLoginUrl", model.AccountLoginUrl }
        };

        emailId = SendEmail(delegatedPerson.Email, _messagingConfig.ToDelegatedPersonDelegatedPersonAccepted, delegatedPersonParameters);
        emailIds.Add(emailId);

        return emailIds;
    }

    public List<string> DelegatedPersonRejected(ApplicationEmailModel model)
    {
        ValidateRequiredApplicationEmailModelParameters(model);
        ValidateStringParameter(model.RejectionComments, nameof(model.RejectionComments));

        var delegatedPerson = model.DelegatedPeople.Single();
        ValidateUserEmailParameters(delegatedPerson);

        var emailIds = new List<string>();

        var approvedPersonParameters = new Dictionary<string, object>
        {
            { "delegatedPerson", $"{delegatedPerson.FirstName} {delegatedPerson.LastName}" },
            { "firstName", model.ApprovedPerson.FirstName },
            { "lastName", model.ApprovedPerson.LastName },
            { "organisationNumber", model.OrganisationNumber.ToReferenceNumberFormat() },
            { "organisationName", model.OrganisationName },
            { "reasonForRejection", model.RejectionComments },
            { "accountLoginUrl", model.AccountLoginUrl }
        };

        var emailId = SendEmail(model.ApprovedPerson.Email, _messagingConfig.ToApprovedPersonDelegatedPersonRejected, approvedPersonParameters);
        emailIds.Add(emailId);

        var delegatedPersonParameters = new Dictionary<string, object>
        {
            { "firstName", delegatedPerson.FirstName },
            { "lastName", delegatedPerson.LastName },
            { "organisationNumber", model.OrganisationNumber.ToReferenceNumberFormat() },
            { "organisationName", model.OrganisationName },
            { "reasonForRejection", model.RejectionComments },
            { "accountLoginUrl", model.AccountLoginUrl },
        };

        emailId = SendEmail(delegatedPerson.Email, _messagingConfig.ToDelegatedPersonDelegatedPersonRejected, delegatedPersonParameters);
        emailIds.Add(emailId);

        return emailIds;
    }

    public List<string> SubmissionAccepted(SubmissionEmailModel model, EventType type)
    {
        ValidateRequiredSubmissionEmailModelParameters(model);

        foreach (var email in model.UserEmails)
        {
            ValidateUserEmailParameters(email);
        }

        var emailIds = new List<string>();
        
        string templateId;

        if (type == EventType.RegulatorPoMDecision)
        {
            templateId = _messagingConfig.RegulatorSubmissionAccepted;
        }
        else
        {
            templateId = _messagingConfig.RegulatorRegistrationAccepted;
        }
        
        foreach (var userEmail in model.UserEmails)
        {
            var userEmailParameters = new Dictionary<string, object>
            {
                { "firstName", userEmail.FirstName },
                { "lastName", userEmail.LastName },
                { "organisationNumber", model.OrganisationNumber.ToReferenceNumberFormat() },
                { "submissionPeriod", model.SubmissionPeriod },
                { "organisationName", model.OrganisationName },
                { "accountLoginUrl", model.AccountLoginUrl }             
            };

            var emailId = SendEmail(userEmail.Email, templateId, userEmailParameters);
            emailIds.Add(emailId);
        }

        return emailIds;
    }

    public List<string> SubmissionRejected(SubmissionEmailModel model, bool? resubmissionRequired)
    {
        ValidateRequiredSubmissionEmailModelParameters(model);
        ValidateStringParameter(model.RejectionComments, nameof(model.RejectionComments));

        string templateId;
        if (resubmissionRequired.HasValue)
        {
            templateId = resubmissionRequired.Value
                ? _messagingConfig.RegulatorSubmissionRejectedResubmissionRequired
                : _messagingConfig.RegulatorSubmissionRejectedResubmissionNotRequired;
        }
        else
        {
            templateId = _messagingConfig.RegulatorRegistrationRejected;
        }

        foreach (var email in model.UserEmails)
        {
            ValidateUserEmailParameters(email);
        }

        var emailIds = new List<string>();

        foreach (var userEmail in model.UserEmails)
        {
            var userEmailParameters = new Dictionary<string, object>
            {
                {"firstName", userEmail.FirstName},
                {"lastName", userEmail.LastName},
                {"organisationNumber", model.OrganisationNumber.ToReferenceNumberFormat()},
                {"organisationName", model.OrganisationName},
                {"submissionPeriod", model.SubmissionPeriod},
                {"reasonForRejection", model.RejectionComments},
                {"accountLoginUrl", model.AccountLoginUrl}
            };

            var emailId = SendEmail(userEmail.Email, templateId, userEmailParameters);
            emailIds.Add(emailId);
        }

        return emailIds;
    }

    public string? SendRemovedApprovedPersonNotification(AssociatedPersonResults model, string notificationType)
    {
        Dictionary<string, object> parameters = null;
        
        switch (notificationType)
        {
            case "PromotedApprovedUser":
            case "RemovedApprovedUser":
                parameters = new Dictionary<string, object>
                {
                    { "email", model.Email },
                    { "firstName", model.FirstName },
                    { "lastName", model.LastName },
                    { "organisationNumber", model.OrganisationId.ToReferenceNumberFormat() },
                    { "companyName", model.CompanyName },
                    { "accountSignInUrl", model.AccountSignInUrl}
                };
               ValidateRequiredRemovedApprovedPersonEmailModelParameters(model);
                break;
           
            case "DemotedDelegatedUsed" : 
                parameters = new Dictionary<string, object>
                {
                    { "email", model.Email },
                    { "firstName", model.FirstName },
                    { "lastName", model.LastName },
                    { "organisationNumber", model.OrganisationId.ToReferenceNumberFormat() }
                };
                ValidateDemotedDelegatedPersonParameters(model);
                break;
        }
       
        var response =  _notificationClient.SendEmail(model.Email, model.TemplateId, parameters);

        return response.id;
    }
    
    public string SendEmailToInvitedNewApprovedPerson(AddRemoveNewApprovedPersonEmailModel model)
    {
        Dictionary<string, object> parameters = null;
        
        parameters = new Dictionary<string, object>
        {
            { "email", model.Email },
            { "firstName", model.FirstName },
            { "lastName", model.LastName },
            { "organisationNumber", model.OrganisationNumber },
            { "inviteLink", model.InviteLink },
            { "companyName", model.CompanyName }
        };
        
        Validate(model);
        
       
        var response =  _notificationClient.SendEmail(model.Email, _messagingConfig.InviteNewApprovedPersonTemplateId, parameters);

        return response.id;
    }

    public string? SendAcceptRejectUserDetailChangeEmailToEprUser(UserDetailsChangeNotificationEmailInput input)
    {
        var parameters = new Dictionary<string, object>();
        parameters.Add("firstName", input.OldFirstName);
        parameters.Add("lastName", input.OldLastName);
        parameters.Add("organisationName", input.OrganisationName);
        parameters.Add("organisationNumber", input.OrganisationNumber);
        parameters.Add("jobTitle", input.OldJobTitle);
        parameters.Add("updatedfirstName", input.NewFirstName);
        parameters.Add("updatedlastName", input.NewLastName);
        parameters.Add("updatedjobTitle", input.NewJobTitle);
        parameters.Add("email", input.ContactEmailAddress);
        parameters.Add("telephoneNumber", input.ContactTelephone);
        parameters.Add("externalId", input.ExternalIdReference);

        var templateId = _messagingConfig.UserDetailChangeRequestTemplateId;

        var recipient = GetEprPackagingRegulatorEmail(input.Nation);

        string? notificationId = null;
        try
        {
            var response = _notificationClient.SendEmail(recipient, templateId, parameters);
            notificationId = response.id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ExceptionLogMessage, input.OrganisationNumber, $"{input.OldFirstName} {input.OldLastName}", templateId);
        }
        return notificationId;
    }

    private void Validate(AddRemoveNewApprovedPersonEmailModel model)
    {
        ValidateStringParameter(model.Email, nameof(model.Email));
        ValidateStringParameter(model.FirstName, nameof(model.FirstName));
        ValidateStringParameter(model.LastName, nameof(model.LastName));
        ValidateStringParameter(model.OrganisationNumber, nameof(model.OrganisationNumber));
        ValidateStringParameter(model.InviteLink, nameof(model.InviteLink));
        ValidateStringParameter(model.CompanyName, nameof(model.CompanyName));
    }

    private void ValidateRequiredRemovedApprovedPersonEmailModelParameters(AssociatedPersonResults model)
    {
        ValidateStringParameter(model.Email, nameof(model.Email));
        ValidateStringParameter(model.FirstName, nameof(model.FirstName));
        ValidateStringParameter(model.LastName, nameof(model.LastName));
        ValidateStringParameter(model.CompanyName, nameof(model.CompanyName));
        ValidateStringParameter(model.OrganisationId, nameof(model.OrganisationId));
        ValidateStringParameter(model.AccountSignInUrl, nameof(model.AccountSignInUrl));
    }

    private void ValidateRequiredApplicationEmailModelParameters(ApplicationEmailModel model)
    {
        ValidateStringParameter(model.ApprovedPerson.Email, nameof(model.ApprovedPerson.Email));
        ValidateStringParameter(model.ApprovedPerson.FirstName, nameof(model.ApprovedPerson.FirstName));
        ValidateStringParameter(model.ApprovedPerson.LastName, nameof(model.ApprovedPerson.LastName));
        ValidateStringParameter(model.OrganisationName, nameof(model.OrganisationName));
        ValidateStringParameter(model.AccountLoginUrl, nameof(model.AccountLoginUrl));
        ValidateStringParameter(model.OrganisationNumber, nameof(model.OrganisationNumber));
    }
    
    private void ValidateRequiredSubmissionEmailModelParameters(SubmissionEmailModel model)
    {
        ValidateStringParameter(model.OrganisationName, nameof(model.OrganisationName));
        ValidateStringParameter(model.AccountLoginUrl, nameof(model.AccountLoginUrl));
        ValidateStringParameter(model.OrganisationNumber, nameof(model.OrganisationNumber));
        ValidateStringParameter(model.SubmissionPeriod, nameof(model.SubmissionPeriod));
    }
    private void ValidateDemotedDelegatedPersonParameters(AssociatedPersonResults model)
    {
        ValidateStringParameter(model.Email, nameof(model.Email));
        ValidateStringParameter(model.FirstName, nameof(model.FirstName));
        ValidateStringParameter(model.LastName, nameof(model.LastName));
        ValidateStringParameter(model.OrganisationId, nameof(model.OrganisationId));
    }
    
    private void ValidateUserEmailParameters(UserEmailModel model)
    {
        ValidateStringParameter(model.Email, nameof(model.Email));
        ValidateStringParameter(model.FirstName, nameof(model.FirstName));
        ValidateStringParameter(model.LastName, nameof(model.LastName));
    }
    
    private void ValidateStringParameter(string parameter, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(parameter))
        {
            throw new ArgumentException("Cannot be empty string", parameterName);
        }
    }

    private string? SendEmail(string recipient, string templateId, Dictionary<string, object> parameters)
    {
        string? notificationId = null;

        try
        {
            var response = _notificationClient.SendEmail(recipient, templateId, parameters);
            notificationId = response.id;
        }
        catch (Exception ex)
        {
            string callingMethodName = (new StackTrace()).GetFrame(1).GetMethod().Name;

            var organisationNumber = parameters.Single(x => x.Key == "organisationNumber").Value.ToString();

            _logger.LogError(ex,
                $"GOV UK NOTIFY ERROR. Class: {this.GetType().Name}, Method: {callingMethodName}, Organisation: {organisationNumber}, Template: {templateId}");
        }

        return notificationId;
    }

    private string GetEprPackagingRegulatorEmail(string nation)
    {
        try
        {
            var timeout = TimeSpan.FromSeconds(timeoutInSeconds);
            nation = Regex.Replace(nation, @"\s", string.Empty, RegexOptions.None, timeout);
            switch (nation)
            {
                case "England":
                    return _eprPackagingRegulatorEmailConfig.England.ToLower();
                case "Scotland":
                    return _eprPackagingRegulatorEmailConfig.Scotland.ToLower();
                case "Wales":
                    return _eprPackagingRegulatorEmailConfig.Wales.ToLower();
                case "NorthernIreland":
                    return _eprPackagingRegulatorEmailConfig.NorthernIreland.ToLower();
                default:
                    throw new ArgumentException("Nation not valid");
            }
        }
        catch (RegexMatchTimeoutException ex)
        {
            _logger.LogError(ex, "Regular Expression timeout out {Nation}", nation);
            throw new ArgumentException("Nation not valid");
        }
    }
}
