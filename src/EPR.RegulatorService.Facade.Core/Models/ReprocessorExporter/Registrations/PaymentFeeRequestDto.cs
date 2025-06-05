namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class PaymentFeeRequestDto
{
    public string RequestorType { get; set; }
    public string Regulator { get; set; }
    public DateTime SubmissionDate { get; set; }
    public string MaterialType { get; set; }
    public string ApplicationReferenceNumber { get; set; } 
}
