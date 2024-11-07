using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;

public class GetRegistrationSubmissionDetailsRequest
{
    [Required]
    public string SubmissionId { get; set; }
}