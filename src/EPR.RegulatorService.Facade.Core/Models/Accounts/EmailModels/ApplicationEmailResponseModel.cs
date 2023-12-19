namespace EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;

public class ApplicationEmailModel
{
    public UserEmailModel ApprovedPerson { get; set; } = new();
    public string OrganisationNumber { get; set; }
    public string OrganisationName { get; set; }
    public string AccountLoginUrl { get; set; }
    public string RejectionComments { get; set; }
    public List<UserEmailModel> DelegatedPeople { get; set; } = new();
}
