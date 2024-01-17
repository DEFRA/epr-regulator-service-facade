namespace EPR.RegulatorService.Facade.Core.Models;

public class RemoveApprovedUsersRequest
{
    public Guid UserId { get; set; }
    
    public Guid OrganisationId { get; set; }
    
    public Guid? RemovedConnectionExternalId { get; set; }

    public Guid? PromotedPersonExternalId { get; set; }
}