using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
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
        var paymentFeeRequest = new PaymentFeeRequestDto
        {
            RequestorType = "Producer",
            Regulator = "NationCode",
            SubmissionDate = DateTime.UtcNow,
            MaterialType = "Plastic",
            ApplicationReferenceNumber = "ABC123"
        };

        // Act
        var result = await _client.GetRegistrationPaymentFee(paymentFeeRequest);

        // Assert
        result.MaterialType.Should().Be("Plastic");
        result.RegistrationFee.Should().Be(2921.00M);
        result.PreviousPaymentDetail.PaymentMode.Should().Be("Offline");
        result.PreviousPaymentDetail.PaymentMethod.Should().Be("Bank transfer (Bacs)");
        result.PreviousPaymentDetail.PaymentDate.Should().Be(DateTime.UtcNow.Date);
        result.PreviousPaymentDetail.PaymentAmount.Should().Be(2900.00M);
    }

    [TestMethod]
    public async Task SaveOfflinePayment_ShouldReturnTrue_WhenCalled()
    {
        // Arrange
        var requestDto = new SaveOfflinePaymentRequestDto();

        // Act
        var result = await _client.SaveOfflinePayment(requestDto);

        // Assert
        result.Should().BeTrue(); 
    }


    [TestMethod]
    public async Task GetAccreditationPaymentFee_ShouldReturnHardcodedValue()
    {
        // Arrange
        var materialName = "Plastic";
        var nationName = "England";
        var submittedDate = DateTime.UtcNow;
        var requestorType = "Producer";
        var reference = "ABC123";

        // Act
        var result = await _client.GetAccreditationPaymentFee(materialName, nationName, submittedDate, requestorType, reference);

        // Assert
        result.Should().Be(3000.00m);
    }

    [TestMethod]
    public async Task SaveAccreditationOfflinePayment_ShouldReturnTrue_WhenCalled()
    {
        // Arrange
        var requestDto = new SaveOfflinePaymentRequestDto();

        // Act
        var result = await _client.SaveAccreditationOfflinePayment(requestDto);

        // Assert
        result.Should().BeTrue();
    }

}
