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
public class ReprocessorExporterServiceClientTests
{
    private Mock<HttpMessageHandler> _mockHttpMessageHandler = null!;
    private Mock<IOptions<PrnBackendServiceApiConfig>> _mockOptions = null!;
    private Mock<ILogger<ReprocessorExporterServiceClient>> _mockLogger = null!;
    private ReprocessorExporterServiceClient _client = null!;
    private Fixture _fixture = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object) { BaseAddress = new Uri("https://mock-api.com/") };

        _mockOptions = new Mock<IOptions<PrnBackendServiceApiConfig>>();
        _mockLogger = new Mock<ILogger<ReprocessorExporterServiceClient>>();
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
                ReprocessingIOByRegistrationMaterialId = "api/v{0}/registrationMaterials/{1}/reprocessingInputsOutputs",
                SamplingPlanByRegistrationMaterialId = "api/v{0}/registrationMaterials/{1}/samplingPlan",
                RegistrationFeeByRegistrationMaterialId = "api/v{0}/registrationMaterials/{1}/paymentFees",
                MarkAsDulyMadeByRegistrationMaterialId = "api/v{0}/registrationMaterials/{1}/markAsDulyMade",
                RegistrationAccreditationReference = "api/v{0}/registrationMaterials/{1}/RegistrationAccreditationReference",
                RegistrationByIdWithAccreditations = "api/v{0}/registrations/{1}/accreditations",
                SamplingPlanByAccreditationId = "api/v{0}/accreditations/{1}/samplingPlan",
                AccreditationFeeByAccreditationMaterialId = "api/v{0}/accreditationMaterials/{1}/paymentFees",
                MarkAsDulyMadeByAccreditationId = "api/v{0}/accreditationMaterials/markAsDulyMade",
                UpdateRegulatorAccreditationTaskStatusById = "api/v{0}/accreditationMaterialTaskStatus"
            }
        });

        _client = new ReprocessorExporterServiceClient(httpClient, _mockOptions.Object, _mockLogger.Object);
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
        var result = await _client.GetRegistrationByRegistrationId(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"));

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
        var result = await _client.GetRegistrationMaterialByRegistrationMaterialId(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"));

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetRegistrationAccreditationReference_ShouldReturnExpectedResult()
    {
        // Arrange
        var id = Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175");
        var expectedDto = _fixture.Create<RegistrationAccreditationReferenceDto>();
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never };
        var responseContent = new StringContent(JsonSerializer.Serialize(expectedDto, jsonOptions));

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(msg =>
                    msg.Method == HttpMethod.Get &&
                    msg.RequestUri!.ToString().Contains($"registrationMaterials/{id}/RegistrationAccreditationReference")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = responseContent });

        // Act
        var result = await _client.GetRegistrationAccreditationReference(id);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task UpdateMaterialOutcomeByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<UpdateMaterialOutcomeWithReferenceDto>();
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
        var result = await _client.UpdateMaterialOutcomeByRegistrationMaterialId(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"), requestDto);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task GetWasteLicenceByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var id = Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175");
        var expectedDto = _fixture.Create<RegistrationMaterialWasteLicencesDto>();
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never };
        var responseContent = new StringContent(JsonSerializer.Serialize(expectedDto, jsonOptions));

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(msg =>
                    msg.Method == HttpMethod.Get &&
                    msg.RequestUri!.ToString().Contains($"registrationMaterials/{id}/wasteLicenses")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = responseContent });

        // Act
        var result = await _client.GetWasteLicenceByRegistrationMaterialId(id);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetReprocessingIOByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var id = Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175");
        var expectedDto = _fixture.Create<RegistrationMaterialReprocessingIODto>();
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never };
        var responseContent = new StringContent(JsonSerializer.Serialize(expectedDto, jsonOptions));

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(msg =>
                    msg.Method == HttpMethod.Get &&
                    msg.RequestUri!.ToString().Contains($"registrationMaterials/{id}/reprocessingInputsOutputs")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = responseContent });

        // Act
        var result = await _client.GetReprocessingIOByRegistrationMaterialId(id);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetSamplingPlanByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var id = Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175");
        var expectedDto = _fixture.Create<RegistrationMaterialSamplingPlanDto>();
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never };
        var responseContent = new StringContent(JsonSerializer.Serialize(expectedDto, jsonOptions));

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(msg =>
                    msg.Method == HttpMethod.Get &&
                    msg.RequestUri!.ToString().Contains($"registrationMaterials/{id}/samplingPlan")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = responseContent });

        // Act
        var result = await _client.GetSamplingPlanByRegistrationMaterialId(id);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }



    [TestMethod]
    public async Task GetSiteAddressByRegistrationId_ShouldReturnExpectedResult()
    {
        // Arrange
        var expectedDto = _fixture.Create<RegistrationSiteAddressDto>();
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
        var result = await _client.GetSiteAddressByRegistrationId(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"));

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetAuthorisedMaterialByRegistrationId_ShouldReturnExpectedResult()
    {
        // Arrange
        var expectedDto = _fixture.Create<MaterialsAuthorisedOnSiteDto>();
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
        var result = await _client.GetAuthorisedMaterialByRegistrationId(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"));

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task MarkAsDulyMadeByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<MarkAsDulyMadeWithUserIdDto>();
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
        var result = await _client.MarkAsDulyMadeByRegistrationMaterialId(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"), requestDto);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task GetRegistrationFeeRequestByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var expectedDto = _fixture.Create<RegistrationFeeContextDto>();
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
        var result = await _client.GetRegistrationFeeRequestByRegistrationMaterialId(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"));

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetAccreditationsByRegistrationId_ShouldReturnExpectedResult()
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
        var result = await _client.GetRegistrationByIdWithAccreditationsAsync(Guid.NewGuid(), 2024);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetSamplingPlanByAccreditationId_ShouldReturnExpectedResult()
    {
        // Arrange
        var id = Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175");
        var expectedDto = _fixture.Create<AccreditationSamplingPlanDto>();
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never };
        var responseContent = new StringContent(JsonSerializer.Serialize(expectedDto, jsonOptions));

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(msg =>
                    msg.Method == HttpMethod.Get &&
                    msg.RequestUri!.ToString().Contains($"accreditations/{id}/samplingPlan")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = responseContent });

        // Act
        var result = await _client.GetSamplingPlanByAccreditationId(id);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }
    
    [TestMethod]
    public async Task GetAccreditationPaymentFeeDetailsByAccreditationId_ShouldReturnExpectedResult()
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
        var result = await _client.GetAccreditationPaymentFeeDetailsByAccreditationId(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"));

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task MarkAccreditationStatusAsDulyMade_ShouldReturnExpectedResult()
    {
        // Arrange
        var accreditationId = Guid.NewGuid();
        var requestDto = _fixture.Create<MarkAsDulyMadeWithUserIdDto>();
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
        var result = await _client.MarkAsDulyMadeByAccreditationId(accreditationId, requestDto);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task UpdateAccreditationMaterialTaskStatus_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<UpdateAccreditationTaskStatusWithUserIdDto>();

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
        var result = await _client.UpdateRegulatorAccreditationTaskStatus(requestDto);
        
        // Assert
        result.Should().BeTrue();
    }
}