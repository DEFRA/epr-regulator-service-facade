using System.ComponentModel.DataAnnotations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;
public class AccreditationPaymentFeeDetailsDto : PaymentFeeDetailsDto
{
    [Required]
    public Guid AccreditationId { get; set; }
}