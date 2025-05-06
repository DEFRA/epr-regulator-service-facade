namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
public class RegistrationSiteAddressDto
{
    public int RegistrationId { get; init; }
    public int NationId { get; set; }
    public string SiteAddress { get; set; } = string.Empty;
    public string GridReference { get; set; } = string.Empty;
    public string LegalCorrespondenceAddress { get; set; } = string.Empty;
}
