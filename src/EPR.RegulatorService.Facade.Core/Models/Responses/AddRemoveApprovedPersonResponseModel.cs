using EPR.RegulatorService.Facade.Core.Models.Organisations;

namespace EPR.RegulatorService.Facade.Core.Models.Responses;

public class AddRemoveApprovedPersonResponseModel
{
    public List<AssociatedPersonResults> DemotedBasicUsers { get; set; } =
        new ();
    public string InviteToken { get; set; }
    
    public string OrganisationReferenceNumber { get; set; }
    
    public string OrganisationName { get; set; }
}