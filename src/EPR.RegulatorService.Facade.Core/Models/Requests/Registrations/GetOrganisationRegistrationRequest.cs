using System;
using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
public class GetOrganisationRegistrationRequest
{
    public Guid UserId { get; set; }
    public string OrganisationName { get; set; }
    public string OrganisationReference { get; set; }
    public string OrganisationType { get; set; }
    public string Statuses { get; set; }
    public string RegistrationYears { get; set; }
    [Required]
    public int? PageNumber { get; set; }
    [Required]
    public int? PageSize { get; set; }
}
