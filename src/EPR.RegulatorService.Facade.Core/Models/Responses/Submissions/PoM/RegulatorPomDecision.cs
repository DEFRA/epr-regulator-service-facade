namespace EPR.RegulatorService.Facade.Core.Models.Responses.Submissions.PoM;

public class RegulatorPomDecision
{
    public Guid FileId { get; set; }
    
    public string? Comments { get; set; }

    public string Decision { get; set; }

    public bool IsResubmissionRequired { get; set; }
}