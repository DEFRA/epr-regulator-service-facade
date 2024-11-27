using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using System.Web;

namespace EPR.RegulatorService.Facade.Core.Extensions;

public static class GetOrganisationRegistrationSubmissionsFilterExtensions
{
    public static string GenerateQueryString(this GetOrganisationRegistrationSubmissionsFilter source)
    {
        static string? convertSpaceToComma(string? input) =>
           string.IsNullOrWhiteSpace(input)
               ? null
               : string.Join(",", input.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        var queryParams = new Dictionary<string, string?>
        {
            { "OrganisationNameCommaSeparated", convertSpaceToComma(source.OrganisationName) },
            { "OrganisationIDCommaSeparated", convertSpaceToComma(source.OrganisationReference) },
            { "RelevantYearCommaSeparated", convertSpaceToComma(source.RelevantYears.ToString()) },
            { "SubmissionStatusCommaSeparated", convertSpaceToComma(source.Statuses) },
            { "OrganisationTypesCommaSeparated", convertSpaceToComma(source.OrganisationType) },
            { "PageNumber", source.PageNumber?.ToString() },
            { "PageSize", source.PageSize?.ToString() }
        };

        // Filter out null or empty values and encode the parameters
        var queryString = string.Join("&",
            queryParams
                .Where(kv => !string.IsNullOrEmpty(kv.Value))
                .Select(kv => $"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode(kv.Value)}"));

        return queryString;
    }
}
