namespace EPR.RegulatorService.Facade.Core.Models.Requests.Submissions;

public class AddRemoveApprovedUserRequest
{
    public Guid OrganisationId { get; set; }
    
    public Guid? RemovedConnectionExternalId { get; set; }

    public string InvitedPersonEmail { get; set; }

    public string AddingOrRemovingUserEmail { get; set; }

    public Guid AddingOrRemovingUserId { get; set; }
}