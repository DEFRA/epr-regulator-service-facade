namespace EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;

public class SubmissionEmailModel
{
    public string OrganisationNumber { get; set; }
    public string OrganisationName { get; set; }
    public string AccountLoginUrl { get; set; }
    public string RejectionComments { get; set; }
    public List<UserEmailModel> UserEmails { get; set; } = new();
}
