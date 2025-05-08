using Azure.Core;
using Azure.Identity;
using EPR.RegulatorService.Facade.Core.Configs;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

namespace EPR.RegulatorService.Facade.API.Handlers;

[ExcludeFromCodeCoverage]
public class PaymentBackendServiceAuthorisationHandler : DelegatingHandler
{
    private readonly TokenRequestContext _tokenRequestContext;
    private readonly DefaultAzureCredential? _credentials;

    public PaymentBackendServiceAuthorisationHandler(IOptions<PaymentBackendServiceApiConfig> options)
    {
        if (!string.IsNullOrEmpty(options.Value.ClientId))
        {
            _tokenRequestContext = new TokenRequestContext(new[] { options.Value.ClientId });
            _credentials = new DefaultAzureCredential();
        }
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await AddDefaultToken(request, cancellationToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task AddDefaultToken(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_credentials != null)
        {
            var tokenResult = await _credentials.GetTokenAsync(_tokenRequestContext, cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue(Microsoft.Identity.Web.Constants.Bearer, tokenResult.Token);
        }
    }
}

