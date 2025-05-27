using EPR.RegulatorService.Facade.Core.Enums;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData;

[ExcludeFromCodeCoverage]
public class OrganisationRegistrationSummaryDto
{
    public Guid SubmissionId { get; set; }
    public Guid OrganisationId { get; set; }
    public string OrganisationType { get; set; }
    public string OrganisationName { get; set; }
    public string OrganisationReference { get; set; }
    public string SubmissionStatus { get; set; }
    public string? StatusPendingDate { get; set; }
    public string ApplicationReferenceNumber { get; set; }
    public string? RegistrationReferenceNumber { get; set; }
    public int RelevantYear { get; set; }
    public string SubmittedDateTime { get; set; }
    public string? RegulatorDecisionDate { get; set; }
    public Guid? RegulatorUserId { get; set; }
    public int NationId { get; set; }
    public bool IsResubmission { get; set; }
    public string? ResubmissionStatus { get; set; }
    public string? ResubmissionDate { get; set; }
    public string? RegistrationDate { get; set; }


    public static implicit operator OrganisationRegistrationSubmissionSummaryResponse(OrganisationRegistrationSummaryDto dto)
    {
        var response = new OrganisationRegistrationSubmissionSummaryResponse
        {
            SubmissionId = dto.SubmissionId,

            OrganisationId = dto.OrganisationId,

            OrganisationName = dto.OrganisationName,

            OrganisationReference = dto.OrganisationReference
        };

        if (!Enum.TryParse<RegistrationSubmissionOrganisationType>(dto.OrganisationType, true, out var organisationType))
        {
            // No need to assign here as the organisationType would have set to default
        }
        response.OrganisationType = organisationType;

        response.ApplicationReferenceNumber = dto.ApplicationReferenceNumber;

        response.RegistrationReferenceNumber = dto.RegistrationReferenceNumber ?? string.Empty;

        if (!DateTime.TryParse(dto.SubmittedDateTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind,out DateTime submissionDate))
        {
            // No need to assign here as the submissionDate would have set to default
        }

        response.SubmissionDate = submissionDate;

        response.RegistrationYear = dto.RelevantYear;

        if (!Enum.TryParse<RegistrationSubmissionStatus>(dto.SubmissionStatus, true, out var submissionStatus))
        {
            submissionStatus = RegistrationSubmissionStatus.None;
        }
        response.SubmissionStatus = submissionStatus;

        response.StatusPendingDate = null;
        if (!string.IsNullOrWhiteSpace(dto.StatusPendingDate))
        {
            response.StatusPendingDate = DateTime.Parse(dto.StatusPendingDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        response.NationId = dto.NationId;

        response.RegistrationDate = null;
        if (!string.IsNullOrWhiteSpace(dto.RegistrationDate))
        {
            response.RegistrationDate = DateTime.Parse(dto.RegistrationDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        response.ResubmissionDate = null;
        if (!string.IsNullOrWhiteSpace(dto.ResubmissionDate))
        {
            response.ResubmissionDate = DateTime.Parse(dto.ResubmissionDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        response.IsResubmission = dto.IsResubmission;

        if (dto.IsResubmission)
        {
            if (!Enum.TryParse<RegistrationSubmissionStatus>(dto.ResubmissionStatus, true, out var resubmissionStatus))
            {
                resubmissionStatus = RegistrationSubmissionStatus.None;
            }
            response.ResubmissionStatus = resubmissionStatus;
        }
        
        if (!string.IsNullOrWhiteSpace(dto.RegulatorDecisionDate))
        {
            response.RegulatorDecisionDate = DateTime.Parse(dto.RegulatorDecisionDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        return response;
    }
}