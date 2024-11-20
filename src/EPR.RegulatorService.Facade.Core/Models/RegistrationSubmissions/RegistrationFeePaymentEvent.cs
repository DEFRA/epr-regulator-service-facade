using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;

public class RegistrationFeePaymentEvent
{
    public Guid SubmissionId { get; set; }
    
    public EventType Type { get; set; } = EventType.RegistrationFeePayment; // NOSONAR

    public string PaymentMethod { get; set; }

    public string PaymentStatus { get; set; }

    public string PaidAmount { get; set; }
}