using System;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class SaveOfflinePaymentRequestDto
{
    public decimal Amount { get; set; }
    public string PaymentReference { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; }
    public string Regulator { get; set; }
    public Guid UserId { get; set; }
    public string Description { get; set; }
    public string? Comments { get; set; }
}
