using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions.Registrations;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;

public class GetRegistrationSubmissionsRequest : RegistrationSubmissionsFilters
{
    public Guid UserId { get; set; }
    
    public RegulatorRegistrationDecision[]? DecisionsDelta { get; set; }
}