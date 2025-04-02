using EPR.RegulatorService.Facade.Core.Clients;
using EPR.RegulatorService.Facade.Core.Configs;
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
    private readonly ILogger<RegistrationServiceClient> _logger = logger;

    public async Task<bool> UpdateRegulatorRegistrationTaskStatus(int id, UpdateTaskStatusRequestDto request)
    {
        _logger.LogInformation("Attempting to update regulator registration task status using the backend for Id {Id} and Status {Status}", id, request.Status.ToString());

        // e.g. v{0}/regulatorRegistrationTaskStatus/{1}
        var url = string.Format(_config.Endpoints.UpdateRegulatorRegistrationTaskStatusById, _config.ApiVersion, id);

        return await PatchAsync(url, request);
    }

    public async Task<bool> UpdateRegulatorApplicationTaskStatus(int id, UpdateTaskStatusRequestDto request)
    {
        _logger.LogInformation("Attempting to update regulator application task status using the backend for Id {Id} and Status {Status}", id, request.Status.ToString());

        // e.g. v{0}/regulatorApplicationTaskStatus/{1}
        var url = string.Format(_config.Endpoints.UpdateRegulatorApplicationTaskStatusById, _config.ApiVersion, id);

        return await PatchAsync(url, request);
    }
}
