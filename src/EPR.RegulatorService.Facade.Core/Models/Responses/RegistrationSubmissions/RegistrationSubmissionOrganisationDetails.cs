using System.Diagnostics.CodeAnalysis;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;

[ExcludeFromCodeCoverage]
public class RegistrationSubmissionOrganisationDetails
{
    public Guid SubmissionId { get; init; }
    public Guid OrganisationId { get; init; }
    public string OrganisationReference { get; init; }
    public string OrganisationName { get; init; }
    public RegistrationSubmissionOrganisationType OrganisationType { get; init; }
    public int NationId { get; init; }
    public string RegistrationYear { get; init; }
    public DateTime RegistrationDateTime { get; init; }
    public RegistrationSubmissionStatus SubmissionStatus { get; init; }
    public DateTime? SubmissionStatusPendingDate { get; set; }
    public string? RegulatorComments { get; set; } = string.Empty;
    public string? ProducerComments { get; set; } = string.Empty;
    public string ApplicationReferenceNumber { get; init; } = string.Empty;
    public string? RegistrationReferenceNumber { get; init; } = string.Empty;
    public string CompaniesHouseNumber { get; set; }
    public string? BuildingName { get; set; }
    public string? SubBuildingName { get; set; }
    public string? BuildingNumber { get; set; }
    public string Street { get; set; }
    public string Locality { get; set; }
    public string? DependentLocality { get; set; }
    public string Town { get; set; }
    public string County { get; set; }
    public string Country { get; set; }
    public string Postcode { get; set; }

    public RegistrationSubmissionOrganisationSubmissionSummaryDetails SubmissionDetails { get; set; }
    public RegistrationSubmissionsOrganisationPaymentDetails PaymentDetails { get; set; }

    public static implicit operator OrganisationRegistrationSubmissionSummaryResponse
        (RegistrationSubmissionOrganisationDetails details) => new ()
    {
        SubmissionId = details.SubmissionId,
        OrganisationId = details.OrganisationId,
        OrganisationName = details.OrganisationName,
        OrganisationType = details.OrganisationType,
        OrganisationReference = details.OrganisationReference,
        RegistrationYear = details.RegistrationYear,
        RegistrationStatus = details.SubmissionStatus,
        StatusPendingDate = details.SubmissionStatusPendingDate,
        ApplicationReferenceNumber = details.ApplicationReferenceNumber,
        RegistrationReferenceNumber = details.RegistrationReferenceNumber,
        NationId = details.NationId,
        SubmissionDate = details.RegistrationDateTime
    };
}