namespace EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;

public class RegistrationSubmissionDetailsResponse
{
    public string? OrganisationRef { get; set; }
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
    public string? CompaniesHouseNumber { get; set; }
    public string? RegulatorComments { get; set; }
    public string? ProducerComments { get; set; }
}