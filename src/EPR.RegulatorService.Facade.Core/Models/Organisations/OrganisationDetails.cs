using EPR.RegulatorService.Facade.Core.Models.Applications;

namespace EPR.RegulatorService.Facade.Core.Models.Organisations;

public class OrganisationDetails
{
    public string OrganisationName { get; set; }
    public string OrganisationId { get; set; }
    public int OrganisationTypeId { get; set; }
    public string? CompaniesHouseNumber { get; set; }
    public bool IsComplianceScheme { get; set; }
    public AddressModel RegisteredAddress { get; set; }
}