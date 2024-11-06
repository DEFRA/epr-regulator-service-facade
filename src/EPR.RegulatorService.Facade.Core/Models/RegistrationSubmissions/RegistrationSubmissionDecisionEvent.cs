using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;

public class RegistrationSubmissionDecisionEvent
{
    public Guid OrganisationId { get; set; }

    public Guid SubmissionId { get; set; }

    /// <summary>
    /// Never make this static as this needs to be passed onto the submission API. If static, this won't be passed into the API
    /// </summary>
    public EventType Type => EventType.RegulatorOrganisationRegistrationDecision;

    public RegulatorDecision Decision { get; set; }

    public string? Comments { get; set; }
}