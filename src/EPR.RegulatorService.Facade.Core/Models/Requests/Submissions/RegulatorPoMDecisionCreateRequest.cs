namespace EPR.RegulatorService.Facade.Core.Models.Requests.Submissions;

public class RegulatorPoMDecisionCreateRequest : AbstractDecisionRequest
{
    public bool IsResubmissionRequired { get; set; }
    
}