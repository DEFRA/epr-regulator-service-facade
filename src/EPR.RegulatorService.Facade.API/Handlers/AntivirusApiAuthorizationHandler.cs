namespace EPR.RegulatorService.Facade.API.Handlers;

using Azure.Core;
using Azure.Identity;
using Core.Configs;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

[ExcludeFromCodeCoverage]
public class AntivirusApiAuthorizationHandler : DelegatingHandler
{
    private readonly TokenRequestContext _tokenRequestContext;
    private readonly ClientSecretCredential _credentials;

    public AntivirusApiAuthorizationHandler(IOptions<AntivirusApiConfig> options)
    {
        var antivirusApiOptions = options.Value;
        _tokenRequestContext = new TokenRequestContext(new[] { antivirusApiOptions.Scope });
        _credentials = new ClientSecretCredential(antivirusApiOptions.TenantId, antivirusApiOptions.ClientId, antivirusApiOptions.ClientSecret);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var tokenResult = await _credentials.GetTokenAsync(_tokenRequestContext, cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue(Constants.Bearer, tokenResult.Token);

        return await base.SendAsync(request, cancellationToken);
    }
}