using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Configs;

[ExcludeFromCodeCoverage]
public class PaymentBackendServiceApiConfig
{
    public const string SectionName = "PaymentBackendServiceApiConfig";

    public string BaseUrl { get; set; } = null!;
    public int Timeout { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public int ServiceRetryCount { get; set; }
    public PaymentServiceApiConfigEndpoints Endpoints { get; set; } = null!;
}

[ExcludeFromCodeCoverage]
public class PaymentServiceApiConfigEndpoints
{
    public string GetRegistrationPaymentFee { get; set; }
}
