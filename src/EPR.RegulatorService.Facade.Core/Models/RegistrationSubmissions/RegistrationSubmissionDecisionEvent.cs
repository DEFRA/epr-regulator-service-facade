using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;

public class RegistrationSubmissionDecisionEvent
{
    public Guid OrganisationId { get; set; }

    public Guid SubmissionId { get; set; }

    public EventType Type { get; set; } = EventType.RegulatorOrganisationRegistrationDecision; // NOSONAR

    public RegulatorDecision Decision { get; set; }

    public string? Comments { get; set; }

    public string? RegistrationReferenceNumber { get; set; }

    public DateTime? DecisionDate { get; set; }
}