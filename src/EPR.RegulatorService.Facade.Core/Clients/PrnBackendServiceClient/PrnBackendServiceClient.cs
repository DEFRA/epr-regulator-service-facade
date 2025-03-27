using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.PrnBackends;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Clients.PrnBackendServiceClient
{
    [ExcludeFromCodeCoverage]
    public class PrnBackendServiceClient(
    HttpClient httpClient,
    IOptions<PrnBackendServiceApiConfig> options,
    ILogger<PrnBackendServiceClient> logger)
    : IPrnBackendServiceClient
    {
        private readonly PrnBackendServiceApiConfig _config = options.Value;

        public async Task<List<PrnBackendModel>> GetAllPrnByOrganisationId(Guid orgId)//This method needs removing when the test is completed. This method is only used for testing
        {
            try
            {
                logger.LogInformation("PrnServiceClient - GetAllPrnByOrganisationId: calling endpoint 'v1/prn/organisation' with organisation id {OrganisationId}", orgId);

                var url = $"{_config.Endpoints.GetAllPrnByOrganisationId}"; 
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("X-EPR-ORGANISATION", orgId.ToString());
                var response = await httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                logger.LogInformation("PrnServiceClient - GetAllPrnsForOrganisation: response from endpoint {Response}", content);

                return JsonConvert.DeserializeObject<List<PrnBackendModel>>(content);
            }
            catch (HttpRequestException exception)
            {
                logger.LogError(exception, "PrnServiceClient - GetAllPrnsForOrganisation: An error occurred retrieving prns for organisation {OrganisationId}", orgId);
                throw;
            }
        }
    }
}
