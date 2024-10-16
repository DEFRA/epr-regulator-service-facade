using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
using EPR.RegulatorService.Facade.Core.Services;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.CommonData.ConcreteData;
using EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData;
using System.Net.Http.Json;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData.ConcreteData;

public class CommonDataOrganisationRegistrationHandler(HttpClient httpClient, CommonDataApiConfig config) : IOrganisationRegistrationDataSource
{
    private readonly HttpClient _httpClient = httpClient
                      ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly CommonDataApiConfig _config = config
                  ?? throw new ArgumentNullException(nameof(config));

    public async Task<HttpResponseMessage> GetOrganisationRegistrations(GetOrganisationRegistrationRequest request)
    {
        var url = string.Format($"{_config.Endpoints.GetOrganisationRegistrations}");
        return await _httpClient.PostAsJsonAsync(url, request);
    }
}
