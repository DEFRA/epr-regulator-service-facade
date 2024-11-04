using EPR.RegulatorService.Facade.Core.Enums;
using System;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.Registrations;
public class RegistrationSubmissionsSummaryResponse
{
    public string? SubmissionId { get; set; }
    public string? OrganisationName { get; set; }
    public string? OrganisationReference { get; set; }
    public string? OrganisationType { get; set; }
    public DateTime RegistrationDateTime { get; set; }
    public string? RegistrationStatus { get; set; }
    public int NationId { get; set; }
    public string ApplicationReferenceNumber { get; set; }
    public string? RegistrationReferenceNumber { get; set; }
}

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
