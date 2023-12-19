using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.Submissions.Events;

public abstract class AbstractEvent
{
    public abstract EventType Type { get; }
}