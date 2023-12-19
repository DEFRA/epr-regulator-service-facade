namespace EPR.RegulatorService.Facade.Core.Models.Applications;

public class OrganisationEnrolments
{
    public Guid OrganisationId { get; set; }
    
    public string OrganisationName { get; set; }
    
    public DateTime LastUpdate { get; set; }
    
    public PendingEnrolmentFlags Enrolments { get; set; }
}
