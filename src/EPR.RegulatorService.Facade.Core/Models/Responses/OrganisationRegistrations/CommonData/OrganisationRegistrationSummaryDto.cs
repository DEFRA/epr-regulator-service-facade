using EPR.RegulatorService.Facade.Core.Enums;
using System.Globalization;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData;

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
#pragma warning disable S3877 // Exceptions should not be thrown from unexpected methods
            throw new InvalidCastException($"Invalid OrganisationType: {dto.OrganisationType}");
#pragma warning restore S3877 // Exceptions should not be thrown from unexpected methods
        }
        response.OrganisationType = organisationType;

        response.ApplicationReferenceNumber = dto.ApplicationReferenceNumber;

        response.RegistrationReferenceNumber = dto.RegistrationReferenceNumber ?? string.Empty;

        response.SubmissionDate = DateTime.Parse(dto.SubmittedDateTime, CultureInfo.InvariantCulture);

        response.RegistrationYear = dto.RelevantYear.ToString();

        if (!Enum.TryParse<RegistrationSubmissionStatus>(dto.SubmissionStatus, true, out var submissionStatus))
        {
            throw new InvalidCastException($"Invalid SubmissionStatus: {dto.SubmissionStatus}");
        }
        response.SubmissionStatus = submissionStatus;

        if (!string.IsNullOrWhiteSpace(dto.StatusPendingDate))
        {
            response.StatusPendingDate = DateTime.Parse(dto.StatusPendingDate, CultureInfo.InvariantCulture);
        }
        else
        {
            response.StatusPendingDate = null;
        }

        response.NationId = dto.NationId;

        if (!string.IsNullOrWhiteSpace(dto.RegulatorCommentDate))
        {
            response.RegulatorCommentDate = DateTime.Parse(dto.RegulatorCommentDate, CultureInfo.InvariantCulture);
        }
        else
        {
            response.RegulatorCommentDate = null;
        }

        if (!string.IsNullOrWhiteSpace(dto.ProducerCommentDate))
        {
            response.ProducerCommentDate = DateTime.Parse(dto.ProducerCommentDate, CultureInfo.InvariantCulture);
        }
        else
        {
            response.ProducerCommentDate = null;
        }

        response.RegulatorUserId = dto.RegulatorUserId;

        return response;
    }
}
