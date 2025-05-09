using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Configs;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.UnitTests.Clients.ReprocessorExporter;

[TestClass]
public class PaymentServiceClientTests
{
    private Mock<IOptions<PaymentBackendServiceApiConfig>> _mockOptions = null!;
    private Mock<ILogger<PaymentServiceClient>> _mockLogger = null!;
    private PaymentServiceClient _client = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://mock-api.com")
        };

        _mockLogger = new Mock<ILogger<PaymentServiceClient>>();
        _mockOptions = new Mock<IOptions<PaymentBackendServiceApiConfig>>();
        _mockOptions.Setup(opt => opt.Value).Returns(new PaymentBackendServiceApiConfig
        {
            Endpoints = new PaymentServiceApiConfigEndpoints
            {
                GetRegistrationPaymentFee = "payment/registration/fee/{0}/{1}/{2}/{3}/{4}"
            }
        });

        _client = new PaymentServiceClient(httpClient, _mockOptions.Object, _mockLogger.Object);
    }

    [TestMethod]
    public async Task GetRegistrationPaymentFee_ShouldReturnHardcodedValue()
    {
        // Arrange
        var materialName = "Plastic";
        var nationName = "England";
        var submittedDate = DateTime.UtcNow;
        var requestorType = "Producer";
        var reference = "ABC123";

        // Act
        var result = await _client.GetRegistrationPaymentFee(materialName, nationName, submittedDate, requestorType, reference);

        // Assert
        result.Should().Be(2921.00m);
    }
}
