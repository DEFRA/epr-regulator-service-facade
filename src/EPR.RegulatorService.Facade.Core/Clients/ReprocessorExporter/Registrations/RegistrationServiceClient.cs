using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Clients;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;

public class RegistrationServiceClient(
HttpClient httpClient,
IOptions<PrnBackendServiceApiConfig> options,
ILogger<RegistrationServiceClient> logger)
: BaseHttpClient(httpClient), IRegistrationServiceClient
{
    private readonly PrnBackendServiceApiConfig _config = options.Value;

    public async Task<RegistrationOverviewDto> GetRegistrationByRegistrationId(int id)
    {
        logger.LogInformation(LogMessages.RegistrationMaterialsTasks);

        var url = string.Format($"{_config.Endpoints.RegistrationByRegistrationId}", _config.ApiVersion, id);

        return await GetAsync<RegistrationOverviewDto>(url);
    }

    public async Task<RegistrationMaterialDetailsDto> GetRegistrationMaterialByRegistrationMaterialId(int id)
    {
        logger.LogInformation(LogMessages.SummaryInfoMaterial);

        var url = string.Format($"{_config.Endpoints.RegistrationMaterialByRegistrationMaterialId}", _config.ApiVersion, id);

        return await GetAsync<RegistrationMaterialDetailsDto>(url);
    }

    public async Task<bool> UpdateMaterialOutcomeByRegistrationMaterialId(int id, UpdateMaterialOutcomeRequestDto request)
    {
        logger.LogInformation(LogMessages.OutcomeMaterialRegistration);

        var url = string.Format($"{_config.Endpoints.UpdateMaterialOutcomeByRegistrationMaterialId}", _config.ApiVersion, id);

        return await PatchAsync(url, request);
    }
}
