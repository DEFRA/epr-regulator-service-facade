using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
public class PaymentFeeDetailsDto
{
    public Guid RegistrationId { get; set; }
    public Guid RegistrationMaterialId { get; set; }
    public string OrganisationName { get; set; }
    public string? SiteAddress { get; set; }
    public string ApplicationReferenceNumber { get; set; }
    public string MaterialName { get; set; }
    public DateTime SubmittedDate { get; set; }
    public decimal FeeAmount { get; set; }
    public ApplicationOrganisationType ApplicationType { get; init; }
    public string Regulator { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? DulyMadeDate { get; set; }
    public DateTime? DeterminationDate { get; set; }
}
