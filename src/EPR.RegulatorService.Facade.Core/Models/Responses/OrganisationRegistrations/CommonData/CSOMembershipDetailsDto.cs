using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData;

[ExcludeFromCodeCoverage]
public class CsoMembershipDetailsDto
{
    public string MemberId { get; set; }
    public string MemberType { get; set; }
    public bool IsOnlineMarketPlace { get; set; }
    public bool IsLateFeeApplicable { get; set; }
    public int NumberOfSubsidiaries { get; set; }

    [JsonPropertyName("NumberOfSubsidiariesOnlineMarketPlace")]
    public int NoOfSubsidiariesOnlineMarketplace { get; set; }
    public int RelevantYear { get; set; }
    public DateTime SubmittedDate { get;set; }
    public string SubmissionPeriodDescription {get;set;}
}
