using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;

public class AccountServiceClient(
HttpClient httpClient,
IOptions<AccountsServiceApiConfig> options,
ILogger<AccountServiceClient> logger)
: BaseHttpClient(httpClient), IAccountServiceClient
{
    private readonly AccountsServiceApiConfig _config = options.Value;
    public async Task<NationDetailsResponseDto> GetNationDetailsById(int nationId)
    {
        logger.LogInformation(LogMessages.AttemptingNationDetails);

        var url = string.Format($"{_config.Endpoints.GetNationDetailsById}", nationId);
        return await GetAsync<NationDetailsResponseDto>(url);
    }

    public async Task<List<PersonDetailsResponseDto>> GetPersonDetailsByIds(PersonDetailsRequestDto requestDto)
    {
        logger.LogInformation(LogMessages.AttemptingPersonDetails);

        var url = string.Format($"{_config.Endpoints.GetPersonDetailsByIds}");
        return await PostAsync<PersonDetailsRequestDto, List<PersonDetailsResponseDto>>(url, requestDto);
    }

    public async Task<OrganisationDetailsResponseDto> GetOrganisationDetailsById(Guid id)
    {
        logger.LogInformation(LogMessages.AttemptingOrganisationDetails);

        var url = string.Format($"{_config.Endpoints.GetOrganisationDetailsById}", id);
        return await GetAsync<OrganisationDetailsResponseDto>(url);
    }
}
