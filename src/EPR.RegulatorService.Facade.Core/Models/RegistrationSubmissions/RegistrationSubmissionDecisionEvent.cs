using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;

public class RegistrationSubmissionDecisionEvent
{
    public Guid OrganisationId { get; set; }

    public Guid SubmissionId { get; set; }

    public EventType Type { get; set; } = EventType.RegulatorRegistrationDecision; // NOSONAR

    public bool IsForOrganisationRegistration { get; set; } = true;

    public RegulatorDecision Decision { get; set; }

    public string? Comments { get; set; }

    public string ApplicationReferenceNumber { get; set; }

    public string? RegistrationReferenceNumber { get; set; }

    public DateTime? DecisionDate { get; set; }
}