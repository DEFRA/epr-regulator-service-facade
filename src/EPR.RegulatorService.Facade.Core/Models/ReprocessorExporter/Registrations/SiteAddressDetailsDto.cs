namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
public class SiteAddressDetailsDto
{
    public int RegistrationId { get; init; }
    public string NationName { get; set; } = string.Empty;
    public string SiteAddress { get; set; } = string.Empty;
    public string GridReference { get; set; } = string.Empty;
    public string LegalCorrespondenceAddress { get; set; } = string.Empty;
}