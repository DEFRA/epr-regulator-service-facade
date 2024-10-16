using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Models.Applications;
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
            var paginatedRegistrations = Paginate<OrganisationRegistrationSummaryResponse>(filteredRegistrations, request.PageNumber.Value, request.PageSize.Value);
            
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(paginatedRegistrations, Encoding.UTF8, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"))
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

    public static List<OrganisationRegistrationData> FilterRegistrations(OrganisationRegistrationDataCollection data, GetOrganisationRegistrationRequest request)
    {
        // Start with the entire collection
        var filteredData = data.Items.AsEnumerable();  // Using AsEnumerable() since we are working with an in-memory collection

        // Filter by OrganisationName if it's provided
        if (!string.IsNullOrWhiteSpace(request.OrganisationName))
        {
            filteredData = filteredData.Where(r => r.OrganisationName != null &&
                                                   r.OrganisationName.Contains(request.OrganisationName, StringComparison.InvariantCultureIgnoreCase));
        }

        // Filter by OrganisationReference if it's provided
        if (!string.IsNullOrWhiteSpace(request.OrganisationReference))
        {
            filteredData = filteredData.Where(r => r.OrganisationReference != null &&
                                                   r.OrganisationReference.Contains(request.OrganisationReference, StringComparison.InvariantCultureIgnoreCase));
        }

        // Filter by OrganisationType if it's provided
        if (!string.IsNullOrWhiteSpace(request.OrganisationType))
        {
            filteredData = filteredData.Where(r => r.OrganisationType != null &&
                                                   request.OrganisationType.Contains(r.OrganisationType.ToString(), StringComparison.InvariantCultureIgnoreCase));
        }

        // Filter by Statuses if provided
        if (!string.IsNullOrWhiteSpace(request.Statuses))
        {
            filteredData = filteredData.Where(r => r.RegistrationStatus != null &&
                                                   request.Statuses.Contains(r.RegistrationStatus.ToString(), StringComparison.InvariantCultureIgnoreCase));
        }

        // Filter by SubmissionYears if provided
        if (!string.IsNullOrEmpty(request.RegistrationYears))
        {
            filteredData = filteredData.Where(r => request.RegistrationYears.Contains(r.RegistrationYear.ToString(), StringComparison.InvariantCultureIgnoreCase));
        }

        // Convert to a list and return
        return filteredData.ToList();
    }

    public static string Paginate<T>(List<T>? items, int currentPage, int pageSize)
    {
        currentPage = currentPage < 1 ? 1 : currentPage;
        object retObj = null;

        if ( items.Count == 0)
        {
            retObj = new PaginatedResponse<T> { Items = [], CurrentPage = currentPage, PageSize = pageSize, TotalItems = 0 };
        } else
        {
            var paginatedItems = items.Skip((currentPage - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToList();
            retObj = new PaginatedResponse<T>
            {
                Items = paginatedItems,
                CurrentPage = currentPage,
                TotalItems = items.Count,
                PageSize = pageSize
            };
        }

        return JsonSerializer.Serialize(retObj);
    }
}
