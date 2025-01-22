using EPR.RegulatorService.Facade.Core.Enums;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;

[ExcludeFromCodeCoverage]
public class PackagingDataResubmissionFeePaymentEvent
{
    public Guid SubmissionId { get; set; }

    public EventType Type { get; set; } = EventType.PackagingDataResubmissionFeePayment;

    public string PaymentMethod { get; set; }

    public string PaymentStatus { get; set; }

    public string PaidAmount { get; set; }
}