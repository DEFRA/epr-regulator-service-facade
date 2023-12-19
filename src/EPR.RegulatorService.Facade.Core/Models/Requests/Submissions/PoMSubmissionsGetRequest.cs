using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.Submissions;

[ExcludeFromCodeCoverage]
public class PoMSubmissionsFilters
{
    public string? OrganisationName { get; set; }
    
    public string? OrganisationReference { get; set; }
    
    public string? OrganisationType { get; set; }
    
    public string? Statuses { get; set; }
    
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }
}