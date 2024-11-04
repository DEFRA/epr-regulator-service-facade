using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;

public class RegistrationSubmissionDecisionCreateRequest : AbstractDecisionRequest
{
    public string? RegulatorComment { get; set; }
}