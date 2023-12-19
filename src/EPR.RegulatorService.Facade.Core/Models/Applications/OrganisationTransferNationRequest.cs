namespace EPR.RegulatorService.Facade.Core.Models.Applications;

public class OrganisationTransferNationRequest
{
    public Guid UserId { get; set; }
    public Guid OrganisationId { get; set; }
    public int TransferNationId { get; set; }
    public string TransferComments { get; set; }
}