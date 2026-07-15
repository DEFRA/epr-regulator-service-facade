using System.Diagnostics.CodeAnalysis;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData;

[ExcludeFromCodeCoverage]
public class CsoMembershipDetailsDto
{
    public string MemberId { get; set; }
    public string MemberType { get; set; }
    public bool IsOnlineMarketPlace { get; set; }
    public bool IsLateFeeApplicable { get; set; }
    public bool IsClosedLoopRecycling { get; set; }

    public int NumberOfSubsidiaries { get; set; }

    public int NumberOfSubsidiariesOnlineMarketPlace { get; set; }

    public int? NumberOfHoldingCompaniesClosedLoopRecycling { get; set; }
    public int? NumberOfSubsidiariesClosedLoopRecycling { get; set; }
    public int RelevantYear { get; set; }
    public DateTime SubmittedDate { get;set; }
    public string SubmissionPeriodDescription {get;set;}

    public static implicit operator CsoMembershipDetailsResponse(CsoMembershipDetailsDto dto) => new()
    {
        MemberId = dto.MemberId,
        MemberType = dto.MemberType,
        IsOnlineMarketPlace = dto.IsOnlineMarketPlace,
        IsLateFeeApplicable = dto.IsLateFeeApplicable,
        IsClosedLoopRecycling = dto.IsClosedLoopRecycling,
        NumberOfSubsidiaries = dto.NumberOfSubsidiaries,
        NumberOfSubsidiariesOnlineMarketPlace = dto.NumberOfSubsidiariesOnlineMarketPlace,
        NumberOfHoldingCompaniesClosedLoopRecycling = dto.NumberOfHoldingCompaniesClosedLoopRecycling ?? 0,
        NumberOfSubsidiariesClosedLoopRecycling = dto.NumberOfSubsidiariesClosedLoopRecycling ?? 0,
        RelevantYear = dto.RelevantYear,
        SubmittedDate = dto.SubmittedDate,
        SubmissionPeriodDescription = dto.SubmissionPeriodDescription
    };
}
