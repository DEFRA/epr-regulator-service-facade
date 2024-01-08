using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace EPR.RegulatorService.Facade.Core.Models.Organisations;

[ExcludeFromCodeCoverage]
public class OrganisationSearchResult
{
    public string OrganisationName { get; set; }
    public bool IsComplianceScheme { get; set; }
    public string OrganisationId { get; set; }
    public Guid ExternalId { get; set; }
    public string CompanyHouseNumber { get; set; }
    public string OrganisationType { get; set; }
}