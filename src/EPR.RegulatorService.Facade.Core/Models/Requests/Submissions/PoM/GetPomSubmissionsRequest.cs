using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions.PoM;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.PoM;

public class GetPomSubmissionsRequest : PoMSubmissionsFilters
{
    public Guid UserId { get; set; }
    
    public RegulatorPomDecision[]? DecisionsDelta { get; set; }
}