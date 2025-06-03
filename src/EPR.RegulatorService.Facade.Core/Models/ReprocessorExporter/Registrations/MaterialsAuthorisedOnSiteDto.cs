namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class MaterialsAuthorisedOnSiteDto
{
    public Guid RegistrationId { get; init; }
    public string OrganisationName { get; set; } = string.Empty;
    public string SiteAddress { get; init; }
    public List<MaterialsAuthorisedOnSiteInfoDto> MaterialsAuthorisation { get; set; } = [];
    public string TaskStatus { get; set; }
    public Guid RegulatorRegistrationTaskStatusId { get; set; }
    public Guid OrganisationId { get; set; }
    public List<QueryNoteResponseDto> QueryNotes { get; set; }
}
