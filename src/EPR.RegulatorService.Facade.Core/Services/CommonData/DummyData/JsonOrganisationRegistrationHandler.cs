using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.Registrations;
using EPR.RegulatorService.Facade.Core.Services;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData;
using System.Net;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData;

public class JsonOrganisationRegistrationHandler(string filePath, IDummyDataLoader<OrganisationRegistrationDataCollection> loader) : IOrganisationRegistrationDataSource
{
    public readonly string _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    private readonly IDummyDataLoader<OrganisationRegistrationDataCollection> _loader = loader ?? throw new ArgumentNullException(nameof(loader));

    public async Task<HttpResponseMessage> GetOrganisationRegistrations(GetOrganisationRegistrationRequest request)
    {
        try
        {
            var registrations = OrganisationRegistrationDummyDataCache.GetOrAdd(_filePath, _loader.LoadData).Value;
            var filteredRegistrations = FilterRegistrations(registrations, request).Select(data => (OrganisationRegistrationSummaryResponse)data).ToList();
            
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(filteredRegistrations), Encoding.UTF8, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"))
            };

            return response;
        }
        catch (Exception ex)
        {
            var errorResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent($"Error: {ex.Message}"),
                ReasonPhrase = "File Read Error"
            };

            return errorResponse;
        }
    }

    private static List<OrganisationRegistrationData> FilterRegistrations(OrganisationRegistrationDataCollection data, GetOrganisationRegistrationRequest request)
    {
        var filteredData = data.Items.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.OrganisationName))
        {
            filteredData = filteredData.Where(r => r.OrganisationName.Contains(request.OrganisationName, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.OrganisationReference))
        {
            filteredData = filteredData.Where(r => r.OrganisationId.Equals(request.OrganisationReference, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.OrganisationType))
        {
            filteredData = filteredData.Where(r => request.OrganisationType.Contains(r.OrganisationType.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Statuses))
        {
            filteredData = filteredData.Where(r => request.Statuses.Contains(r.RegistrationStatus.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        if(!string.IsNullOrEmpty(request.SubmissionYears))
        {
            filteredData = filteredData.Where(r => request.SubmissionYears.Contains(r.RegistrationYear.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        return [.. filteredData];
    }
}
