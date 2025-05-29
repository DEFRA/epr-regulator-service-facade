namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class MaterialsAuthorisedOnSiteDto
{
    public Guid RegistrationId { get; init; }
    public string OrganisationName { get; set; } = string.Empty;
    public string SiteAddress { get; init; }
    public List<MaterialsAuthorisedOnSiteInfoDto> MaterialsAuthorisation { get; set; } = [];
    public string TaskStatus { get; set; }
    public Guid RegulatorApplicationTaskStatusId { get; set; }
    public Guid OrganisationId { get; set; }
}
