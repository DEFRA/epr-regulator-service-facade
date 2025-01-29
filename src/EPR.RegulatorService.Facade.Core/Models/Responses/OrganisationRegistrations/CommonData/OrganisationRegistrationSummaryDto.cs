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
    public string? RegulatorCommentDate { get; set; }
    public string? ProducerCommentDate { get; set; }
    public Guid? RegulatorUserId { get; set; }
    public int NationId { get; set; }
    public bool IsResubmission { get; set; }

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

        if (!DateTime.TryParse(dto.SubmittedDateTime, CultureInfo.InvariantCulture, out DateTime submissionDate))
        {
            // No need to assign here as the submissionDate would have set to default
        }

        response.SubmissionDate = submissionDate;

        response.RegistrationYear = dto.RelevantYear;

        if (!Enum.TryParse<RegistrationSubmissionStatus>(dto.SubmissionStatus, true, out var submissionStatus))
        {
            throw new InvalidCastException($"Invalid SubmissionStatus: {dto.SubmissionStatus}");
        }
        response.SubmissionStatus = submissionStatus;

        response.StatusPendingDate = null;
        if (!string.IsNullOrWhiteSpace(dto.StatusPendingDate))
        {
            response.StatusPendingDate = DateTime.Parse(dto.StatusPendingDate, CultureInfo.InvariantCulture);
        }

        response.NationId = dto.NationId;

        response.RegulatorCommentDate = null;
        if (!string.IsNullOrWhiteSpace(dto.RegulatorCommentDate))
        {
            response.RegulatorCommentDate = DateTime.Parse(dto.RegulatorCommentDate, CultureInfo.InvariantCulture);
        }

        response.ProducerCommentDate = null;
        if (!string.IsNullOrWhiteSpace(dto.ProducerCommentDate))
        {
            response.ProducerCommentDate = DateTime.Parse(dto.ProducerCommentDate, CultureInfo.InvariantCulture);
        }

        response.RegulatorUserId = dto.RegulatorUserId;
        response.IsResubmission = dto.IsResubmission;

        return response;
    }
}