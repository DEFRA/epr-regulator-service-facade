using System.Diagnostics.CodeAnalysis;
using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Clients;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;

[ExcludeFromCodeCoverage]
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
        _logger.LogInformation("Attempting to update regulator registration task status using the backend");

        // e.g. regulatorRegistrationTaskStatus/{0}
        var url = string.Format(_config.Endpoints.RegulatorRegistrationTaskStatus, id);

        return await PatchAsync(url, request);
    }

    public async Task<bool> UpdateRegulatorApplicationTaskStatus(int id, UpdateTaskStatusRequestDto request)
    {
        _logger.LogInformation("Attempting to update regulator application task status using the backend");

        // e.g. regulatorApplicationTaskStatus/{0}
        var url = string.Format(_config.Endpoints.RegulatorApplicationTaskStatus, id);

        return await PatchAsync(url, request);
    }
}
