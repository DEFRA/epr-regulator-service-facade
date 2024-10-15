using System;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
public class GetOrganisationRegistrationRequest
{
    public Guid UserId { get; set; }
    public string OrganisationName { get; set; }
    public string OrganisationReference { get; set; }
    public string OrganisationType { get; set; }
    public int? PageNumber { get; set; } = 1;
    public string Statuses { get; set; }
    public string RegistrationYears { get; set; }
    public int? PageSize { get; set; } = 20;
}
