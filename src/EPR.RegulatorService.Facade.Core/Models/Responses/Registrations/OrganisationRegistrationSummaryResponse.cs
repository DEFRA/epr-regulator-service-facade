using EPR.RegulatorService.Facade.Core.Enums;
using System;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.Registrations;
public class OrganisationRegistrationSummaryResponse
{
    public string OrganisationName { get; set; }
    public string OrganisationId { get; set; }
    public string OrganisationReference { get; set; }
    public OrganisationType OrganisationType { get; set; }
    public DateTime RegistrationDateTime { get; set; }
    public int RegistrationYear { get; set; }
    public RegistrationStatus RegistrationStatus { get; set; }
    public int NationId { get; set; }
    public string? CompaniesHouseNumber { get; set; }
    public string? RegulatorComments { get; set; } = string.Empty;
    public string? ProducerComments { get; set; } = string.Empty;
    public string ApplicationReferenceNumber { get; set; } = String.Empty;
    public string? RegistrationReferenceNumber { get; set; } = String.Empty;

    public string? BuildingName { get; set; }
    public string SubBuildingName { get; set; }
    public string? BuildingNumber { get; set; }
    public string? Street { get; set; }
    public string? Locality { get; set; }
    public string? DependantLocality { get; set; }
    public string? Town { get; set; }
    public string? County { get; set; }
    public string? Country { get; set; }
    public string? PostCode { get; set; }
}
