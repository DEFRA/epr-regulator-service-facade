namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class PaymentFeeResponseDto
{
    public string MaterialType { get; set; }
    public decimal RegistrationFee { get; set; }
    public PreviousPaymentDetailDto? PreviousPaymentDetail { get; set; }
}
