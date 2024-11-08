using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;

[ExcludeFromCodeCoverage]
public class GetRegistrationSubmissionDetailsRequest
{
    [Required]
    public string SubmissionId { get; set; }
}