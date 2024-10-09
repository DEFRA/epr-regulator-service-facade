using EPR.RegulatorService.Facade.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
public class OrganisationRegistrationFilter
{
    public string? OrganisationName { get; set; }

    // TODO: Clarify if this is a reference, as per our UI or
    // is this the actual ID as implemented elsewhere - what is the client expecting
    public string? OrganisationReference { get; set; }

    public OrganisationType? OrganisationType { get; set; }

    public RegistrationStatus Statuses { get; set; }

    public string? SubmissionYears { get; set; }

    // TODO: Clarify if this is required, it was, and if so, it changes the scope of dummy data considerably
    public string? SubmissionPeriods { get; set; }

    [Required]
    public int? PageNumber { get; set; }

    [Required]
    public int? PageSize { get; set; }
}
