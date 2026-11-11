using Microsoft.Extensions.Configuration;

namespace IntegrationTests.Infrastructure
{
    public static class ConfigBuilder
    {
        public static IConfiguration GenerateConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            
            // Add in-memory configuration for test settings
            // This can be extended with additional test-specific configuration
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                // Add any default test configuration here if needed
                ["MessagingConfig:ApiKey"] =
                    "integration-test-key-00000000-0000-0000-0000-000000000000-00000000-0000-0000-0000-000000000000"
            });
            
            return configurationBuilder.Build();
        }
    }
}
