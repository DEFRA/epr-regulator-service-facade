using EPR.RegulatorService.Facade.Core.Attributes;
using EPR.RegulatorService.Facade.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;

public class RegistrationSubmissionDecisionCreateRequest
{
    [NotDefault]
    public Guid SubmissionId { get; set; }

    [Required]
    public RegistrationStatus Status { get; set; }

    public string? Comments { get; set; }

    public Guid OrganisationId { get; set; }

    public Guid? UserId { get; set; }
}
