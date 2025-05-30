using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using System;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class RegistrationFeeContextDto
{
    public Guid RegistrationId { get; set; }
    public Guid OrganisationId { get; set; }
    public int NationId { get; set; }
    public string MaterialName { get; set; }
    public string ApplicationReferenceNumber { get; set; }
    public ApplicationOrganisationType ApplicationType { get; init; }
    public string SiteAddress { get; set; } 
    public DateTime CreatedDate { get; set; }
    public DateTime? DulyMadeDate { get; set; }
    public DateTime? DeterminationDate { get; set; }
}
