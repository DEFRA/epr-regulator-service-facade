namespace EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.PoM;

public class RegulatorPoMDecisionCreateRequest : AbstractDecisionRequest
{
    public bool IsResubmissionRequired { get; set; }
}