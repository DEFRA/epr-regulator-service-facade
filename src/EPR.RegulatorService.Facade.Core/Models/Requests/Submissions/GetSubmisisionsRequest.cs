using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.Submissions;

public class PoMSubmissionsGetRequest : PoMSubmissionsFilters
{
    public Guid UserId { get; set; }
    
    public RegulatorPomDecision[]? DecisionsDelta { get; set; }
}