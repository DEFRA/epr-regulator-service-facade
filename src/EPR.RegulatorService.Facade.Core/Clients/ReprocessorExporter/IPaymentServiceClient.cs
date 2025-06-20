using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;

public interface IPaymentServiceClient
{
    Task<PaymentFeeResponseDto> GetRegistrationPaymentFee(PaymentFeeRequestDto request);
    Task<bool> SaveOfflinePayment(SaveOfflinePaymentRequestDto request);
    Task<decimal> GetAccreditationPaymentFee(string materialName, string regulator, DateTime submittedDate, string requestorType, string reference);
    Task<bool> SaveAccreditationOfflinePayment(SaveOfflinePaymentRequestDto request);
}