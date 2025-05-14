using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using System;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class RegistrationFeeContextDto
{
    public int RegistrationId { get; set; }
    public int OrganisationId { get; set; }
    public int NationId { get; set; }
    public string MaterialName { get; set; }
    public string Reference { get; set; }
    public ApplicationOrganisationType ApplicationType { get; init; }
    public string SiteAddress { get; set; } 
    public DateTime CreatedDate { get; set; }
}
