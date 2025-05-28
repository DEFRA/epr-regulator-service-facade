using System.Net;
using System.Text.Json;
using AutoFixture;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Accreditations;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace EPR.RegulatorService.Facade.UnitTests.Clients.ReprocessorExporter.Accreditations;

[TestClass]
public class AccreditationServiceClientTests
{
    private Mock<HttpMessageHandler> _mockHttpMessageHandler = null!;
    private Mock<IOptions<PrnBackendServiceApiConfig>> _mockOptions = null!;
    private Mock<ILogger<AccreditationServiceClient>> _mockLogger = null!;
    private AccreditationServiceClient _client = null!;
    private Fixture _fixture = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object) { BaseAddress = new Uri("https://mock-api.com/") };

        _mockOptions = new Mock<IOptions<PrnBackendServiceApiConfig>>();
        _mockLogger = new Mock<ILogger<AccreditationServiceClient>>();
        _mockOptions.Setup(opt => opt.Value).Returns(new PrnBackendServiceApiConfig
        {
            BaseUrl = "https://mock-api.com",
            Timeout = 30,
            ClientId = "test-client",
            ApiVersion = 1,
            ServiceRetryCount = 3,
            Endpoints = new PrnServiceApiConfigEndpoints
            {   
                AccreditationFeeByRegistrationMaterialId = "api/v{0}/accreditationMaterials/{1}/paymentFees",
                AccreditationMarkAsDulyMadeByRegistrationMaterialId = "api/v{0}/accreditationMaterials/markAsDulyMade",
                UpdateAccreditationMaterialTaskStatus = "api/v{0}/accreditationMaterialTaskStatus"
            }
        });

        _client = new AccreditationServiceClient(httpClient, _mockOptions.Object, _mockLogger.Object);
        _fixture = new Fixture();
    }


    [TestMethod]
    public async Task GetAccreditationFeeRequestByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var expectedDto = _fixture.Create<AccreditationFeeContextDto>();
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
        };
        var responseContent = new StringContent(JsonSerializer.Serialize(expectedDto, jsonOptions));

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = responseContent
            });

        // Act
        var result = await _client.GetAccreditationFeeRequestByRegistrationMaterialId(Guid.NewGuid());

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }


    [TestMethod]
    public async Task MarkAsDulyMadeByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<AccreditationMarkAsDulyMadeWithUserIdDto>();
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
        };
        var responseContent = new StringContent(JsonSerializer.Serialize(true, jsonOptions));

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = responseContent
            });

        // Act
        var result = await _client.MarkAccreditationMaterialStatusAsDulyMade(requestDto);

        // Assert
        result.Should().BeTrue();
    }


    [TestMethod]
    public async Task UpdateAccreditationMaterialTaskStatus_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<UpdateAccreditationMaterialTaskStatusWithUserIdDto>();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(msg =>
                    msg.Method == HttpMethod.Post &&
                    msg.RequestUri!.ToString().Contains("accreditationMaterialTaskStatus")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("true") });


        // Act
        var result = await _client.UpdateAccreditationMaterialTaskStatus(requestDto);

        // Assert
        result.Should().BeTrue();
    }


}