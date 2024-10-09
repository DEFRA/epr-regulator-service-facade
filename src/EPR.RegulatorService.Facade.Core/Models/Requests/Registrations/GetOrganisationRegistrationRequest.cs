using System;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
public class GetOrganisationRegistrationRequest
{
    public Guid UserId { get; set; }
    public string OrganisationName { get; set; }
    public string OrganisationReference { get; set; }
    public string OrganisationType { get; set; }
    public int? PageNumber { get; set; }
    public string Statuses { get; set; }
    public string SubmissionYears { get; set; }
    public string SubmissionPeriods { get; set; }
    public int? PageSize { get; set; }
}
