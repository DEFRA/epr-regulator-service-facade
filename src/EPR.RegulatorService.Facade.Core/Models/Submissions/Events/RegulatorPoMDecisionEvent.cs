using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.Submissions.Events;

public class RegulatorPoMDecisionEvent : AbstractEvent
{
    public override EventType Type => EventType.RegulatorPoMDecision;

    public RegulatorDecision Decision { get; set; }
    
    public string? Comments { get; set; }
    
    public bool IsResubmissionRequired { get; set; }
    
    public Guid FileId { get; set; }
}