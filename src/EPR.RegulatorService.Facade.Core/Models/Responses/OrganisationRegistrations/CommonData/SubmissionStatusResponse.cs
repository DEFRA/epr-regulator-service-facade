using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData;

[ExcludeFromCodeCoverage]
public class SubmissionStatusResponse
{
    public Guid SubmissionId { get; set; }

    public string RegistrationReferenceNumber { get; set; } = string.Empty;

    public string SubmissionStatus { get; set; } = string.Empty;

    public DateTime StatusPendingDate { get; set; }

    public DateTime SubmittedDateTime { get; set; }

    public bool IsResubmission { get; set; }

    public string ResubmissionStatus { get; set; } = string.Empty;

    public string ResubmissionDate { get; set; } = string.Empty;
}