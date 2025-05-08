using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;
public interface IPaymentServiceClient
{
    Task<decimal> GetRegistrationPaymentFee(RegistrationPaymentFeeRequestDto request);
}
