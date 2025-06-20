using Azure.Identity;
using EPR.RegulatorService.Facade.Core.Configs;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.API.Handlers;

[ExcludeFromCodeCoverage]
public class PrnBackendServiceAuthorisationHandler : AzureAuthorizationHandlerBase
{
    public PrnBackendServiceAuthorisationHandler(IOptions<PrnBackendServiceApiConfig> options)
        : base(
            options.Value.ClientId,
            new DefaultAzureCredential())
    {
    }
}