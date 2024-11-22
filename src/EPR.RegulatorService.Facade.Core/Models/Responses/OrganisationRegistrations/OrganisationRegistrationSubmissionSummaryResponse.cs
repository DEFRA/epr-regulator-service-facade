using EPR.RegulatorService.Facade.Core.Enums;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;

[ExcludeFromCodeCoverage]
public class OrganisationRegistrationSubmissionSummaryResponse
{
    public Guid SubmissionId { get; set; }

    public Guid OrganisationId { get; set; }

    public string OrganisationName { get; set; }

    public string OrganisationReference { get; set; }

    public RegistrationSubmissionOrganisationType OrganisationType { get; set; }

    public string ApplicationReferenceNumber { get; set; }

    public string RegistrationReferenceNumber { get; set; }

    public DateTime SubmissionDate { get; set; }

    public string RegistrationYear { get; set; }

    public RegistrationSubmissionStatus SubmissionStatus { get; set; }

    public DateTime? StatusPendingDate { get; set; }

    public int NationId { get; set; }
    public DateTime? RegulatorCommentDate { get; internal set; }
    public DateTime? ProducerCommentDate { get; internal set; }
    public Guid? RegulatorUserId { get; internal set; }
}