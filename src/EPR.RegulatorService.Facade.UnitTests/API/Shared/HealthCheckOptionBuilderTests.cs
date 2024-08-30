using EPR.RegulatorService.Facade.API.HealthChecks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EPR.RegulatorService.Facade.UnitTests.API.Shared;

[TestClass]
public class HealthCheckOptionBuilderTests
{
    [TestMethod]
    public void Build_ShouldReturnHealthCheckOptions_WithExpectedValues()
    {
        // Act
        var result = HealthCheckOptionBuilder.Build();

        // Assert
        result.AllowCachingResponses.Should().BeFalse();
        result.ResultStatusCodes.Should().ContainKey(HealthStatus.Healthy)
            .WhoseValue.Should().Be(StatusCodes.Status200OK);
    }
}