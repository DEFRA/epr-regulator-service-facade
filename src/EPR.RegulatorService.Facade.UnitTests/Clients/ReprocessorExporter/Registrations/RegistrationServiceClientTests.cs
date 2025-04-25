using AutoFixture;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace EPR.RegulatorService.Facade.UnitTests.Clients.ReprocessorExporter.Registrations;

[TestClass]
public class RegistrationServiceClientTests
{
    private Mock<HttpMessageHandler> _mockHttpMessageHandler = null!;
    private Mock<IOptions<PrnBackendServiceApiConfig>> _mockOptions = null!;
    private Mock<ILogger<RegistrationServiceClient>> _mockLogger = null!;
    private RegistrationServiceClient _client = null!;
    private Fixture _fixture = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object) { BaseAddress = new Uri("https://mock-api.com/") };

        _mockOptions = new Mock<IOptions<PrnBackendServiceApiConfig>>();
        _mockLogger = new Mock<ILogger<RegistrationServiceClient>>();
        _mockOptions.Setup(opt => opt.Value).Returns(new PrnBackendServiceApiConfig
        {
            BaseUrl = "https://mock-api.com",
            Timeout = 30,
            ClientId = "test-client",
            ApiVersion = 1,
            ServiceRetryCount = 3,
            Endpoints = new PrnServiceApiConfigEndpoints
            {
                UpdateRegulatorApplicationTaskStatusById = "v{0}/regulatorApplicationTaskStatus",
                UpdateRegulatorRegistrationTaskStatusById = "v{0}/regulatorRegistrationTaskStatus",
                RegistrationByRegistrationId = "registrations/{0}",
                RegistrationMaterialByRegistrationMaterialId = "materials/{0}",
                UpdateMaterialOutcomeByRegistrationMaterialId = "update/material/{0}",
                WasteLicensesByRegistrationMaterialId = "api/v{0}/registrationMaterials/{1}/wasteLicenses",
                ReprocessingInputsOutputsByRegistrationMaterialId = "api/v{0}/registrationMaterials/{1}/reprocessingInputsOutputs",
                SamplingPlanByRegistrationMaterialId = "api/v{0}/registrationMaterials/{1}/samplingPlan"
            }
        });

        _client = new RegistrationServiceClient(httpClient, _mockOptions.Object, _mockLogger.Object);
        _fixture = new Fixture();
    }

    [TestMethod]
    public async Task UpdateRegulatorApplicationTaskStatus_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<UpdateRegulatorApplicationTaskDto>();
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(msg =>
                    msg.Method == HttpMethod.Post &&
                    msg.RequestUri!.ToString().Contains("regulatorApplicationTaskStatus")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("true") });

        // Act
        var result = await _client.UpdateRegulatorApplicationTaskStatus(requestDto);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task UpdateRegulatorRegistrationTaskStatus_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<UpdateRegulatorRegistrationTaskDto>();
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(msg =>
                    msg.Method == HttpMethod.Post &&
                    msg.RequestUri!.ToString().Contains("regulatorRegistrationTaskStatus")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("true") });

        // Act
        var result = await _client.UpdateRegulatorRegistrationTaskStatus(requestDto);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task GetRegistrationByRegistrationId_ShouldReturnExpectedResult()
    {
        // Arrange
        var expectedDto = _fixture.Create<RegistrationOverviewDto>();
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never };
        var responseContent = new StringContent(JsonSerializer.Serialize(expectedDto, jsonOptions));
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = responseContent });

        // Act
        var result = await _client.GetRegistrationByRegistrationId(1);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetRegistrationMaterialByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var expectedDto = _fixture.Create<RegistrationMaterialDetailsDto>();
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never };
        var responseContent = new StringContent(JsonSerializer.Serialize(expectedDto, jsonOptions));
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = responseContent });

        // Act
        var result = await _client.GetRegistrationMaterialByRegistrationMaterialId(1);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task UpdateMaterialOutcomeByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<UpdateMaterialOutcomeRequestDto>();
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
        var result = await _client.UpdateMaterialOutcomeByRegistrationMaterialId(1, requestDto);

        // Assert
        result.Should().BeTrue();
    }
}