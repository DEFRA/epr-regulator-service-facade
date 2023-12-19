namespace EPR.RegulatorService.Facade.Core.Models.Accounts;

public class GovNotificationRequestModel
{
    public string Decision { get; set; }
    public string OrganisationName { get; set; }
    public string OrganisationNumber { get; set; }
    public UserRequestModel ApprovedUser { get; set; } = new();
    public List<UserRequestModel> DelegatedUsers { get; set; } = new();
    public string RejectionComments { get; set; }
    public string RegulatorRole { get; set; }
}
