using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData;

[ExcludeFromCodeCoverage]
public class CsoMembershipDetailsDto
{
    public int MemberId { get; set; }
    public string MemberType { get; set; }
    public bool IsOnlineMarketPlace { get; set; }
    public bool IsLateFeeApplicable { get; set; }
    public int NumberOfSubsidiaries { get; set; }
    public int NoOfSubsidiariesOnlineMarketplace { get; set; }
    public int RelevantYear { get; set; }
    public DateTime SubmittedDate { get;set; }
    public string SubmissionPeriodDescription {get;set;}
}
