using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;

[ExcludeFromCodeCoverage]
public class RegistrationSubmissionsOrganisationPaymentDetails
{
    public decimal ApplicationProcessingFee { get; init; }

    public decimal OnlineMarketplaceFee { get; init; }

    public decimal SubsidiaryFee { get; init; }

    public decimal TotalChargeableItems => ApplicationProcessingFee + OnlineMarketplaceFee + SubsidiaryFee;

    public decimal PreviousPaymentsReceived { get; init; }

    public decimal TotalOutstanding => TotalChargeableItems - PreviousPaymentsReceived;

    public decimal? OfflinePaymentAmount { get; set; }
}