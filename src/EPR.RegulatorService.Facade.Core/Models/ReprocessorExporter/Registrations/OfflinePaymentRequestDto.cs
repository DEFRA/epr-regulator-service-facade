using System;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class OfflinePaymentRequestDto
{
    public decimal Amount { get; set; }
    public string PaymentReference { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; }
    public string Regulator { get; set; }
}