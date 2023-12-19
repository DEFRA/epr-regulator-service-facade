namespace EPR.RegulatorService.Facade.Core.Models.Applications;

public class ApplicationEnrolmentDetailsResponse
{
    public Guid OrganisationId { get; set; }

    public string OrganisationName { get; set; }
    
    public string OrganisationReferenceNumber { get; set; }

    public string OrganisationType { get; set; }

    public string CompaniesHouseNumber { get; set; }

    public bool IsComplianceScheme { get; set; }

    public int? NationId { get; set; }

    public string NationName { get; set; }

    public AddressModel BusinessAddress { get; set; }

    public IEnumerable<UserEnrolmentDetails> Users { get; set; }
    public TransferDetails TransferDetails { get; set; }
}
