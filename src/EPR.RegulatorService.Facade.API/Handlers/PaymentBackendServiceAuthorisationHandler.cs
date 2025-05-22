using Azure.Identity;
using EPR.RegulatorService.Facade.Core.Configs;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.API.Handlers;

[ExcludeFromCodeCoverage]
public class PaymentBackendServiceAuthorisationHandler : AzureAuthorizationHandlerBase
{
    public PaymentBackendServiceAuthorisationHandler(IOptions<PaymentBackendServiceApiConfig> options)
        : base(
            options.Value.ClientId,
            new DefaultAzureCredential())
    {
    }
}

