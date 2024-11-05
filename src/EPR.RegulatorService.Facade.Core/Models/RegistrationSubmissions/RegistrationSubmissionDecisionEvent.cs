using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;

public class RegistrationSubmissionDecisionEvent
{
    public Guid OrganisationId { get; set; }

    public Guid SubmissionId { get; set; }

    public static EventType Type => EventType.RegulatorRegistrationDecision;

    public RegulatorDecision Decision { get; set; }

    public string? RegulatorComment { get; set; }
}