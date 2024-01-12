namespace EPR.RegulatorService.Facade.Core.Models;

public class FacadeAddRemoveApprovedPersonRequest
{
    public Guid OrganisationId { get; set; }
    public Guid? RemovedConnectionExternalId { get; set; }
    public string InvitedPersonEmail { get; set; }
    public string InvitedPersonFirstName { get; set; }
    public string InvitedPersonLastName { get; set; }
    
}