namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class RegistrationAccreditationReferenceDto
{
    public string RegistrationType { get; set; }
    public string OrgCode { get; set; }
    public string MaterialCode { get; set; }
    public string ApplicationType { get; set; }
    public string RandomDigits { get; set; } 
    public string RelevantYear { get; set; } 
    public int NationId { get; set; }
}
