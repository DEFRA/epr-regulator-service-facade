using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;

[ExcludeFromCodeCoverage]
public class RegistrationSubmissionsFilters
{
    public string? OrganisationName { get; init; }
    
    public string? OrganisationReference { get; init; }
    
    public string? OrganisationType { get; init; }
    
    public string? Statuses { get; init; }

    public string? SubmissionYears { get; init; }

    public string? SubmissionPeriods { get; init; }

    [Required]
    public int? PageNumber { get; init; }

    [Required]
    public int? PageSize { get; init; }
}