using EPR.RegulatorService.Facade.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
public class OrganisationRegistrationFilter
{
    public string? OrganisationName { get; set; }

    public string? OrganisationReference { get; set; }

    public OrganisationType? OrganisationType { get; set; }

    public RegistrationStatus Statuses { get; set; }

    public string? RegistrationYear { get; set; }

    [Required]
    public int? PageNumber { get; set; }

    [Required]
    public int? PageSize { get; set; }
}
