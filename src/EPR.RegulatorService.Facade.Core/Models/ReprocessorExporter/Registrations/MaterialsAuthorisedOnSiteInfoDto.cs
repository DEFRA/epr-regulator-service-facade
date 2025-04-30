namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
public class MaterialsAuthorisedOnSiteInfoDto
{
    public string MaterialName { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public bool IsMaterialRegistered { get; set; }
}