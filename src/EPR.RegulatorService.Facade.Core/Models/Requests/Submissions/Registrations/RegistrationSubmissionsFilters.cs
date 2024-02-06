using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;

[ExcludeFromCodeCoverage]
public class RegistrationSubmissionsFilters
{
    public string? OrganisationName { get; set; }
    
    public string? OrganisationReference { get; set; }
    
    public string? OrganisationType { get; set; }
    
    public string? Statuses { get; set; }
    
    [Required]
    public int? PageNumber { get; set; }

    [Required]
    public int? PageSize { get; set; }
}