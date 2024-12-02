using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;

[ExcludeFromCodeCoverage]
public class GetOrganisationRegistrationSubmissionsFilter
{
    public string? OrganisationName { get; set; }
    public string? OrganisationReference { get; set; }
    public string? OrganisationType { get; set; }
    public string? Statuses { get; set; }
    public string? RelevantYears { get; set; }
    public string? ApplicationReferenceNumber { get; set; }
    public string? RegistrationReferenceNumber { get; set; }

    [Required]
    [Range(1, 4, ErrorMessage = "The nationId must be valid")]
    public int NationId { get; set; }
    [Required]
    public int? PageNumber { get; set; } = 1;
    
    [Required]
    public int? PageSize { get; set; } = 20;
}