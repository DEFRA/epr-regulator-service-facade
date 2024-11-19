using EPR.RegulatorService.Facade.Core.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;

[ExcludeFromCodeCoverage]
public class RegistrationFeePaymentCreateRequest
{
    [NotDefault]
    public Guid SubmissionId { get; init; }

    [Required]
    public string PaymentMethod { get; init; }

    [Required]
    public string PaymentStatus { get; init; }

    [Required]
    public string PaidAmount { get; init; }

    public Guid? UserId { get; init; }
}