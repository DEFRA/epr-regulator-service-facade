using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;

public class PaymentServiceClient(
HttpClient httpClient,
IOptions<PaymentBackendServiceApiConfig> options,
ILogger<PaymentServiceClient> logger)
: BaseHttpClient(httpClient), IPaymentServiceClient
{
    private readonly PaymentBackendServiceApiConfig _config = options.Value;
    public async Task<decimal> GetRegistrationPaymentFee(string materialName, string regulator, DateTime submittedDate, string requestorType, string reference)
    {
         logger.LogInformation(LogMessages.AttemptingRegistrationPaymentFee);
         return 2921.00M;

        //var url = string.Format($"{_config.Endpoints.GetRegistrationPaymentFee}", _config.ApiVersion, materialName, regulator, submittedDate, requestorType, reference);
        //return await GetAsync<decimal>(url);
    }

    public async Task<decimal> GetAccreditationPaymentFee(string materialName, string regulator, DateTime submittedDate, string requestorType, string reference)
    {
        logger.LogInformation(LogMessages.AttemptingAccreditationPaymentFee);
        return 3000.00M;

        //var url = string.Format($"{_config.Endpoints.GetAccreditationPaymentFee}", _config.ApiVersion, materialName, regulator, submittedDate, requestorType, reference);
        //return await GetAsync<decimal>(url);
    }

    public async Task<bool> SaveOfflinePayment(SaveOfflinePaymentRequestDto request)
    {
        logger.LogInformation(LogMessages.SaveOfflinePayment);
        return true;

        //var url = string.Format(_config.Endpoints.SaveOfflinePayment, _config.ApiVersion);
        //return await PostAsync<OfflinePaymentRequestDto, bool>(url, request);
    }

    public async Task<bool> SaveAccreditationOfflinePayment(SaveOfflinePaymentRequestDto request)
    {
        logger.LogInformation(LogMessages.SaveAccreditationOfflinePayment);
        return true;

        //var url = string.Format(_config.Endpoints.SaveAccreditationOfflinePayment, _config.ApiVersion);
        //return await PostAsync<SaveOfflinePaymentRequestDto, bool>(url, request);
    }
}
