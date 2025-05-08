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
    public async Task<decimal> GetRegistrationPaymentFee(RegistrationPaymentFeeRequestDto request)
    {
         logger.LogInformation(LogMessages.RegistrationPaymentFee);
         return 2921.00M;

        //var url = string.Format($"{_config.Endpoints.GetRegistrationPaymentFee}", request);
        //return await GetAsync<decimal>(url);
    }
}
