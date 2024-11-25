namespace EPR.RegulatorService.Facade.Core.Configs;

public class MessagingConfig
{
    public const string SectionName = "MessagingConfig";
    
    public string ApiKey { get; set; } = string.Empty;
    
    public string ToApprovedPersonApprovedPersonAccepted { get; set; } = string.Empty;
    public string ToApprovedPersonDelegatedPersonAccepted { get; set; } = string.Empty;
    public string ToDelegatedPersonDelegatedPersonAccepted { get; set; } = string.Empty;
    public string ToApprovedPersonApprovedPersonRejected { get; set; } = string.Empty;
    public string ToApprovedPersonDelegatedPersonRejected { get; set; } = string.Empty;
    public string ToDelegatedPersonDelegatedPersonRejected { get; set; } = string.Empty;
    public string ToDelegatedPersonApprovedPersonRejected { get; set; } = string.Empty;
    public string RegulatorSubmissionAccepted { get; set; } = string.Empty;
    public string RegulatorRegistrationAccepted { get; set; } = string.Empty;
    public string RegulatorRegistrationRejected { get; set; } = string.Empty;
    public string RegulatorSubmissionRejectedResubmissionRequired { get; set; } = string.Empty;
    public string RegulatorSubmissionRejectedResubmissionNotRequired { get; set; } = string.Empty;
    public string AccountSignInUrl { get; set; } = string.Empty;
    public string RemovedApprovedUserTemplateId { get; set; } = string.Empty;
    public string DemotedDelegatedUserTemplateId { get; set; } = string.Empty;
    public string PromotedApprovedUserTemplateId { get; set; } = string.Empty;
    public string InviteNewApprovedPersonTemplateId { get; set; } = string.Empty;
    public string AccountCreationUrl { get; set; } = string.Empty;

    public string OrganisationRegistrationSubmissionQueriedId { get; set; } = string.Empty;
    public string OrganisationRegistrationSubmissionDecisionId { get; set; } = string.Empty; 
    public string WelshOrganisationRegistrationSubmissionQueriedId { get; set; } = string.Empty;
    public string WelshOrganisationRegistrationSubmissionDecisionId { get; set; } = string.Empty; 
}
