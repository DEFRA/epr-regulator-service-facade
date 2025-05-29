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
    //private readonly PaymentBackendServiceApiConfig _config = options.Value;
    public async Task<PaymentFeeResponseDto> GetRegistrationPaymentFee(PaymentFeeRequestDto request)
    {
        logger.LogInformation(LogMessages.AttemptingRegistrationPaymentFee);
        var paymentFeeResponse = new PaymentFeeResponseDto
        {
            MaterialType = "MaterialType",
            RegistrationFee = 2921.00M,
            PreviousPaymentDetail = new PreviousPaymentDetailDto
            {
                PaymentMode = "PaymentMode",
                PaymentMethod = "MaterialType",
                PaymentDate = DateTime.Now,
                PaymentAmount = 2900.00M
            }
        };
        return paymentFeeResponse; 

        //var url = string.Format($"{_config.Endpoints.GetRegistrationPaymentFee}", _config.ApiVersion, materialName, regulator, submittedDate, requestorType, reference);
        //return await GetAsync<decimal>(url);
    }

    public async Task<bool> SaveOfflinePayment(SaveOfflinePaymentRequestDto request)
    {
        logger.LogInformation(LogMessages.SaveOfflinePayment);
        return true;

        //var url = string.Format(_config.Endpoints.SaveOfflinePayment, _config.ApiVersion);
        //return await PostAsync<OfflinePaymentRequestDto, bool>(url, request);
    }
}
