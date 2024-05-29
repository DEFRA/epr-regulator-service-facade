using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Configs
{
    [ExcludeFromCodeCoverage]
    public class AntivirusApiConfig
    {
        public const string SectionName = "AntivirusApi";

        public string BaseUrl { get; set; }

        public string SubscriptionKey { get; set; }

        public string TenantId { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Scope { get; set; }

        public int Timeout { get; set; }

        public bool EnableDirectAccess { get; set; } = false;

        public string CollectionSuffix { get; set; }

        public bool PersistFile { get; set; }
    }
}

