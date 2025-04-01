using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Clients.ReprocessorExporter.Registrations;

[TestClass]
public class RegistrationServiceClientTests
{
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private HttpClient _httpClient;
    private Mock<IOptions<PrnBackendServiceApiConfig>> _optionsMock;
    private Mock<ILogger<RegistrationServiceClient>> _loggerMock;
    private PrnBackendServiceApiConfig _config;

    private RegistrationServiceClient _client;

    [TestInitialize]
    public void Setup()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://fake.api")
        };

        _loggerMock = new Mock<ILogger<RegistrationServiceClient>>();

        _config = new PrnBackendServiceApiConfig
        {
            Endpoints = new PrnServiceApiConfigEndpoints
            {
                RegulatorRegistrationTaskStatus = "regulatorRegistrationTaskStatus/{0}",
                RegulatorApplicationTaskStatus = "regulatorApplicationTaskStatus/{0}"
            }
        };

        _optionsMock = new Mock<IOptions<PrnBackendServiceApiConfig>>();
        _optionsMock.Setup(o => o.Value).Returns(_config);

        _client = new RegistrationServiceClient(_httpClient, _optionsMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task UpdateRegulatorRegistrationTaskStatus_ReturnsTrue_OnSuccess()
    {
        // Arrange
        var dto = new UpdateTaskStatusRequestDto { Status = RegistrationTaskStatus.Complete };
        var expectedUrl = "https://fake.api/regulatorRegistrationTaskStatus/123";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Patch && req.RequestUri.ToString() == expectedUrl),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent)) // success
            .Verifiable();

        // Act
        var result = await _client.UpdateRegulatorRegistrationTaskStatus(123, dto);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeTrue();
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Patch &&
                    req.RequestUri.ToString() == expectedUrl),
                ItExpr.IsAny<CancellationToken>());
        }
    }

    [TestMethod]
    public async Task UpdateRegulatorRegistrationTaskStatus_ReturnsFalse_OnFailure()
    {
        // Arrange
        var dto = new UpdateTaskStatusRequestDto { Status = RegistrationTaskStatus.Complete };
        var expectedUrl = "https://fake.api/regulatorRegistrationTaskStatus/123";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Patch && req.RequestUri.ToString() == expectedUrl),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network failure"));

        // Act
        Func<Task> act = async () => await _client.UpdateRegulatorRegistrationTaskStatus(123, dto);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("Network failure");
    }

    [TestMethod]
    public async Task UpdateRegulatorApplicationTaskStatus_ReturnsTrue_OnSuccess()
    {
        // Arrange
        var dto = new UpdateTaskStatusRequestDto { Status = RegistrationTaskStatus.Complete };
        var expectedUrl = "https://fake.api/regulatorApplicationTaskStatus/123";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Patch && req.RequestUri.ToString() == expectedUrl),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent)) // success
            .Verifiable();

        // Act
        var result = await _client.UpdateRegulatorApplicationTaskStatus(123, dto);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeTrue();
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Patch &&
                    req.RequestUri.ToString() == expectedUrl),
                ItExpr.IsAny<CancellationToken>());
        }
    }

    [TestMethod]
    public async Task UpdateRegulatorApplicationTaskStatus_ReturnsFalse_OnFailure()
    {
        // Arrange
        var dto = new UpdateTaskStatusRequestDto { Status = RegistrationTaskStatus.Complete };
        var expectedUrl = "https://fake.api/regulatorApplicationTaskStatus/123";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Patch && req.RequestUri.ToString() == expectedUrl),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network failure"));

        // Act
        Func<Task> act = async () => await _client.UpdateRegulatorApplicationTaskStatus(123, dto);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("Network failure");
    }

}
