using EPR.RegulatorService.Facade.Core.Enums;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        return new OrganisationRegistrationSubmissionSummaryResponse
        {
            SubmissionId = dto.SubmissionId,
            OrganisationId = dto.OrganisationId,
            OrganisationName = dto.OrganisationName,
            OrganisationReference = dto.OrganisationReference,
            OrganisationType = Enum.TryParse<RegistrationSubmissionOrganisationType>(
                dto.OrganisationType, true, out var organisationType)
                ? organisationType
                : throw new InvalidCastException($"Invalid OrganisationType: {dto.OrganisationType}"),
            ApplicationReferenceNumber = dto.ApplicationReferenceNumber,
            RegistrationReferenceNumber = dto.RegistrationReferenceNumber ?? string.Empty, // Handle null
            SubmissionDate = DateTime.Parse(dto.SubmittedDateTime, CultureInfo.InvariantCulture), // Convert string to DateTime
            RegistrationYear = dto.RelevantYear.ToString(), 
            SubmissionStatus = Enum.TryParse<RegistrationSubmissionStatus>(
                dto.SubmissionStatus, true, out var submissionStatus)
                ? submissionStatus
                : throw new InvalidCastException($"Invalid SubmissionStatus: {dto.SubmissionStatus}"),
            StatusPendingDate = !string.IsNullOrWhiteSpace(dto.StatusPendingDate)
                ? DateTime.ParseExact(dto.StatusPendingDate, "yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture)
                : null, // Handle null or empty values
            NationId = dto.NationId
        };
    }
}
