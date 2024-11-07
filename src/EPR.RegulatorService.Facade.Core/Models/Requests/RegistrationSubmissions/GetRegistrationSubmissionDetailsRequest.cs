using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;

public class GetRegistrationSubmissionDetailsRequest
{
    [Required]
    public Guid SubmissionId { get; set; }

    [Required]
    public Guid OrganisationId { get; set; }

    public string OrganisationReference { get; set; }
}