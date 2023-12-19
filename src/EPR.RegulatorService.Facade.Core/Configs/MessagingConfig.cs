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
    public string RegulatorSubmissionRejectedResubmissionRequired { get; set; } = string.Empty;
    public string RegulatorSubmissionRejectedResubmissionNotRequired { get; set; } = string.Empty;
    public string AccountSignInUrl { get; set; } = string.Empty;
}
