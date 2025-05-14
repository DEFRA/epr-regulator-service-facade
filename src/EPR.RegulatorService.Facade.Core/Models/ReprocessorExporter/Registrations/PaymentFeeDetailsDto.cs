using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
public class PaymentFeeDetailsDto
{
    public int RegistrationId { get; set; }
    public int RegistrationMaterialId { get; set; }
    public string OrganisationName { get; set; }
    public string SiteAddress { get; set; }
    public string ReferenceNumber { get; set; }
    public string MaterialName { get; set; }
    public DateTime SubmittedDate { get; set; }
    public decimal FeeAmount { get; set; }
    public ApplicationOrganisationType ApplicationType { get; init; }
    public string Regulator { get; set; }
}
