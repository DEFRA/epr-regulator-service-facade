using EPR.RegulatorService.Facade.Core.Enums;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
public class OrganisationRegistrationFilter
{
    public string? OrganisationName { get; set; }

    public string? OrganisationReference { get; set; }

    public OrganisationType? OrganisationType { get; set; }

    public RegistrationStatus? Statuses { get; set; }

    public string? RegistrationYears { get; set; }

    [Required]
    public int? PageNumber { get; set; }

    [Required]
    public int? PageSize { get; set; }

    public static implicit operator GetOrganisationRegistrationRequest(OrganisationRegistrationFilter filter)
    {
        if (filter == null) return null;

        return new GetOrganisationRegistrationRequest
        {
            OrganisationName = filter.OrganisationName,
            OrganisationReference = filter.OrganisationReference,
            OrganisationType = filter.OrganisationType != Core.Enums.OrganisationType.none ? filter.OrganisationType.ToString() : string.Empty,
            Statuses = filter.Statuses != Core.Enums.RegistrationStatus.none ? filter.Statuses.ToString() : string.Empty,
            RegistrationYears = filter.RegistrationYears,
            PageSize = filter.PageSize,
            PageNumber = filter.PageNumber
        };
    }
}
