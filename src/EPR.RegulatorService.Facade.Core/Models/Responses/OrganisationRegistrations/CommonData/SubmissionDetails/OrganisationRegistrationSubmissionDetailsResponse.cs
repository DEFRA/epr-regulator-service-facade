using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData.SubmissionDetails
{
    [ExcludeFromCodeCoverage]
    public class OrganisationRegistrationSubmissionDetailsResponse
    {
        public Guid SubmissionId { get; set; }
        public Guid OrganisationId { get; set; }
        public string OrganisationReference { get; set; }
        public string OrganisationName { get; set; }
        public RegistrationSubmissionOrganisationType OrganisationType { get; set; }
        public int NationId { get; set; }
        public string NationCode { get; set; }

        public int RelevantYear { get; set; }
        public DateTime SubmissionDate { get; set; }
        public RegistrationSubmissionStatus SubmissionStatus { get; set; }
        public RegistrationSubmissionStatus ResubmissionStatus { get; set; }
        public DateTime? ResubmissionDate { get; set; }
        public DateTime? StatusPendingDate { get; set; }
        public bool IsResubmission { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string? RegulatorComments { get; set; } = string.Empty;
        public string? ProducerComments { get; set; } = string.Empty;
        public string ApplicationReferenceNumber { get; set; } = string.Empty;
        public string? RegistrationReferenceNumber { get; set; } = string.Empty;
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
        public DateTime? RegulatorDecisionDate { get; internal set; }
        public DateTime? ProducerCommentDate { get; internal set; }
        public Guid? RegulatorUserId { get; internal set; }
        public bool IsOnlineMarketPlace { get; internal set; }
        public int NumberOfSubsidiaries { get; internal set; }
        public int NumberOfOnlineSubsidiaries { get; internal set; }
        public bool IsLateSubmission { get; internal set; }
        public string OrganisationSize { get; internal set; }
        public bool IsComplianceScheme { get; internal set; }
        public string SubmissionPeriod { get; internal set; }
        public List<CsoMembershipDetailsDto> CsoMembershipDetails { get; set; }
        public string? ResubmissionFileId { get; internal set; }
    }
}
