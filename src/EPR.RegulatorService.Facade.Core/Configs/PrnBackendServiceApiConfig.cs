using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Configs
{
    [ExcludeFromCodeCoverage]
    public class PrnBackendServiceApiConfig
    {
        public const string SectionName = "PrnBackendServiceApiConfig";

        public string BaseUrl { get; set; } = null!;
        public int Timeout { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public int ServiceRetryCount { get; set; }
        public PrnServiceApiConfigEndpoints Endpoints { get; set; } = null!;
    }
}

public class PrnServiceApiConfigEndpoints
{
    public string GetAllPrnByOrganisationId { get; set; } = null!;// This endpoint needs removing when the test is completed. This endpoint is only used for testing
}
