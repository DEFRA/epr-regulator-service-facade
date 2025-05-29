namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
public class RegistrationSiteAddressDto
{
    public Guid RegistrationId { get; init; }
    public int NationId { get; set; }
    public string SiteAddress { get; set; } = string.Empty;
    public string GridReference { get; set; } = string.Empty;
    public string LegalCorrespondenceAddress { get; set; } = string.Empty;
    public string TaskStatus { get; set; }
    public Guid RegulatorApplicationTaskStatusId { get; set; }
    public Guid OrganisationId { get; set; }
    public List<QueryNoteResponseDto> QueryNotes { get; set; }
}
