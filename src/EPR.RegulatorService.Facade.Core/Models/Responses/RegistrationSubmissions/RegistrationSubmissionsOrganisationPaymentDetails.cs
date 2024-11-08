
using System.Diagnostics.CodeAnalysis;
using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Models;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;

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