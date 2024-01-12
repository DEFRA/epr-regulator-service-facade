namespace EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;

public class AddRemoveNewApprovedPersonEmailModel
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string OrganisationNumber { get; set; }
    
    public string CompanyName { get; set; }
    
    public string InviteLink { get; set; }
}