using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.Submissions.Events
{
    public class RegulatorRegistrationDecisionEvent : AbstractEvent
    {
        public override EventType Type => EventType.RegulatorRegistrationDecision;

        public RegulatorDecision Decision { get; set; }
    
        public string? Comments { get; set; }
    
        public Guid FileId { get; set;  }
    }
}