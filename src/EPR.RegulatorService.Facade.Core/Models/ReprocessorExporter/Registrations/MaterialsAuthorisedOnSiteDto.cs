namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
public class MaterialsAuthorisedOnSiteDto
{
    public string OrganisationName { get; init; }
    public string SiteAddress { get; init; }
    public List<MaterialsAuthorisedOnSiteInfoDto> MaterialsAuthorisation { get; set; } = [];
}
