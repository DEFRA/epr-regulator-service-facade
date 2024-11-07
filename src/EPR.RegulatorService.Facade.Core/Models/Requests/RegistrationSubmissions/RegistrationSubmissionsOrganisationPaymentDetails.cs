
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;

[ExcludeFromCodeCoverage]
public class RegistrationSubmissionsOrganisationPaymentDetails
{
    public decimal ApplicationProcessingFee { get; set; }

    public decimal OnlineMarketplaceFee { get; set; }

    public decimal SubsidiaryFee { get; set; }

    public decimal TotalChargeableItems => ApplicationProcessingFee + OnlineMarketplaceFee + SubsidiaryFee;

    public decimal PreviousPaymentsReceived { get; set; }

    public decimal TotalOutstanding => TotalChargeableItems - PreviousPaymentsReceived;

    public decimal? OfflinePaymentAmount { get; set; }
}