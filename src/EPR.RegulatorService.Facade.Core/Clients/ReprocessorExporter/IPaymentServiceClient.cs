namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;

public interface IPaymentServiceClient
{
    Task<decimal> GetRegistrationPaymentFee(string materialName, string nationName, DateTime submittedDate, string requestorType, string reference);
}
