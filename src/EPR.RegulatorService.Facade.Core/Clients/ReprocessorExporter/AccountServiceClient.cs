using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Clients;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;

public class AccountServiceClient(
HttpClient httpClient,
IOptions<AccountsServiceApiConfig> options,
ILogger<AccountServiceClient> logger)
: BaseHttpClient(httpClient), IAccountServiceClient
{
    private readonly AccountsServiceApiConfig _config = options.Value;
    private bool IsNationServiceReady => false;//will be deleted when ready
    public async Task<string> GetNationNameById(int id)
    {
        logger.LogInformation(LogMessages.SiteAddressDetails);

        if (!IsNationServiceReady)//will be deleted when ready
        {
            return id switch
            {
                1 => "England",
                2 => "Northern Ireland",
                3 => "Scotland",
                4 => "Wales",
                _ => "Unknown Nation"
            };
        }

        var url = string.Format($"{_config.Endpoints.GetNationNameById}", id);
        return await GetAsync<string>(url);
    }
    public async Task<string> GetOrganisationNameById(int id)
    {
        logger.LogInformation(LogMessages.SiteAddressDetails);

        if (!IsNationServiceReady)//will be deleted when ready
        {
            return "Green Ltd";
        }

        var url = string.Format($"{_config.Endpoints.GetOrganisationNameById}", id);
        return await GetAsync<string>(url);
    }
}
