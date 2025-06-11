using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Clients;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
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
    public async Task<NationDetailsResponseDto> GetNationDetailsById(int id)
    {
        logger.LogInformation(LogMessages.AttemptingSiteAddressDetails);

        return id switch
        {
            1 => new NationDetailsResponseDto { Name = "England", NationCode = "GB-ENG" },
            2 => new NationDetailsResponseDto { Name = "Northern Ireland", NationCode = "GB-NIR" },
            3 => new NationDetailsResponseDto { Name = "Scotland", NationCode = "GB-SCT" },
            4 => new NationDetailsResponseDto { Name = "Wales", NationCode = "GB-WLS" },
            _ => new NationDetailsResponseDto { Name = "Unknown Nation", NationCode = "" }
        };

        //var url = string.Format($"{_config.Endpoints.GetNationDetailsById}", id);
        //return await GetAsync<string>(url);
    }

    public async Task<OrganisationDetailsResponseDto> GetOrganisationDetailsById(Guid id)
    {
        logger.LogInformation(LogMessages.AttemptingOrganisationDetails);

        var url = string.Format($"{_config.Endpoints.GetOrganisationDetailsById}", id);
        return await GetAsync<OrganisationDetailsResponseDto>(url);
    }
}
