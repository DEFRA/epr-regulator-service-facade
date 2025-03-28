using EPR.RegulatorService.Facade.Core.Configs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Clients.PrnBackendServiceClient;

[ExcludeFromCodeCoverage]
public class PrnBackendServiceClient(
HttpClient httpClient,
IOptions<PrnBackendServiceApiConfig> options,
ILogger<PrnBackendServiceClient> logger)
: IPrnBackendServiceClient
{
    private readonly PrnBackendServiceApiConfig _config = options.Value;
}
