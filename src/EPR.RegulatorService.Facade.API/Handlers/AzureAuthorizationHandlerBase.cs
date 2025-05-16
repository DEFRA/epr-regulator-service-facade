using Azure.Core;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

namespace EPR.RegulatorService.Facade.API.Handlers;

[ExcludeFromCodeCoverage]
public abstract class AzureAuthorizationHandlerBase : DelegatingHandler
{
    private readonly TokenRequestContext _tokenRequestContext;
    private readonly TokenCredential _credential;
    private readonly string _scheme;

    protected AzureAuthorizationHandlerBase(string serviceClientId, TokenCredential credential, string scheme = Microsoft.Identity.Web.Constants.Bearer)
    {
        if (!string.IsNullOrEmpty(serviceClientId))
        {
            _tokenRequestContext = new TokenRequestContext(new[] { serviceClientId });
            _credential = credential;
            _scheme = scheme;
        }
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_credential != null)
        {
            var tokenResult = await _credential.GetTokenAsync(_tokenRequestContext, cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue(_scheme, tokenResult.Token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
