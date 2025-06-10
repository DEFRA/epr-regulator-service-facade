namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class PreviousPaymentDetailDto
{
    public string PaymentMode { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal PaymentAmount { get; set; }
}
