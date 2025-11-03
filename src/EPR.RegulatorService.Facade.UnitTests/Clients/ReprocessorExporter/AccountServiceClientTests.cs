using AutoFixture;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.UnitTests.Clients.ReprocessorExporter;

[TestClass]
public class AccountServiceClientTests
{
    private Mock<HttpMessageHandler> _mockHttpMessageHandler = null!;
    private Mock<IOptions<AccountsServiceApiConfig>> _mockOptions = null!;
    private Mock<ILogger<AccountServiceClient>> _mockLogger = null!;
    private AccountServiceClient _client = null!;
    private Fixture _fixture = null!;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
    };

    [TestInitialize]
    public void TestInitialize()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://mock-api.com")
        };

        _mockLogger = new Mock<ILogger<AccountServiceClient>>();
        _mockOptions = new Mock<IOptions<AccountsServiceApiConfig>>();
        _mockOptions.Setup(opt => opt.Value).Returns(new AccountsServiceApiConfig
        {
            Endpoints = new AccountsServiceEndpoints
            {
                GetNationDetailsById = "nations/nation-id/{0}",
                GetPersonDetailsByIds = "organisations/person-details-by-ids",
                GetOrganisationDetailsById = "organisations/organisation-with-persons/{0}"
            }
        });

        _client = new AccountServiceClient(httpClient, _mockOptions.Object, _mockLogger.Object);
        _fixture = new Fixture();
    }

    [TestMethod]
    public async Task GetNationNameById_ReturnsValidValue()
    {
        // Arrange
        int nationId = 1;
        var expectedDto = _fixture.Create<NationDetailsResponseDto>();
        var responseContent = new StringContent(JsonSerializer.Serialize(expectedDto, JsonSerializerOptions));
        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
        "SendAsync",
            ItExpr.Is<HttpRequestMessage>(msg =>
                msg.Method == HttpMethod.Get &&
                msg.RequestUri!.ToString().Contains($"nations/nation-id/{nationId}")),
            ItExpr.IsAny<CancellationToken>()
        )
        .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = responseContent });

        // Act
        var result = await _client.GetNationDetailsById(nationId);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }


    [TestMethod]
    public async Task GetNationDetailsById_WithMalformedEndpointConfig_ThrowsFormatException()
    {
        // Arrange
        _mockOptions.Setup(opt => opt.Value).Returns(new AccountsServiceApiConfig
        {
            Endpoints = new AccountsServiceEndpoints
            {
                GetNationDetailsById = "nations/nation-id/" // Missing {0}
            }
        });

        var client = new AccountServiceClient(new HttpClient(_mockHttpMessageHandler.Object), _mockOptions.Object, _mockLogger.Object);
        
        // Act and Assert
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.GetNationDetailsById(1));
    }

    [TestMethod]
    public async Task GetNationDetailsById_WithoutEndpointConfig_ThrowsFormatException()
    {
        // Arrange
        _mockOptions.Setup(opt => opt.Value).Returns(new AccountsServiceApiConfig
        {
            Endpoints = new AccountsServiceEndpoints()
        });
        var client = new AccountServiceClient(new HttpClient(_mockHttpMessageHandler.Object), _mockOptions.Object, _mockLogger.Object);

        // Act and Assert
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.GetNationDetailsById(1));
    }

    [TestMethod]
    public async Task GetPersonDetailsByIds_ReturnsValidValue()
    {
        // Arrange
        var requestDto = _fixture.Create<PersonDetailsRequestDto>();
        var expectedResponseDto = _fixture.Create<List<PersonDetailsResponseDto>>();
        var responseContent = new StringContent(JsonSerializer.Serialize(expectedResponseDto, JsonSerializerOptions));
        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
        "SendAsync",
            ItExpr.Is<HttpRequestMessage>(msg =>
                msg.Method == HttpMethod.Post &&
                msg.RequestUri!.ToString().Contains($"organisations/person-details-by-ids")),
            ItExpr.IsAny<CancellationToken>()
        )
        .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = responseContent });

        // Act
        var result = await _client.GetPersonDetailsByIds(requestDto);


        // Assert
        result.Should().BeEquivalentTo(expectedResponseDto);
    }

    [TestMethod]
    public async Task GetPersonDetailsByIds_WithMalformedEndpointConfig_ThrowsFormatException()
    {
        // Arrange
        var requestDto = _fixture.Create<PersonDetailsRequestDto>();
        _mockOptions.Setup(opt => opt.Value).Returns(new AccountsServiceApiConfig
        {
            Endpoints = new AccountsServiceEndpoints
            {
                GetPersonDetailsByIds = "organisations/person-details-by-ids/1"
            }
        });
                var client = new AccountServiceClient(new HttpClient(_mockHttpMessageHandler.Object), _mockOptions.Object, _mockLogger.Object);
        
        // Act and Assert
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.GetPersonDetailsByIds(requestDto));
    }

    [TestMethod]
    public async Task GetPersonDetailsByIds_WithoutEndpointConfig_ThrowsFormatException()
    {
        // Arrange
        var requestDto = _fixture.Create<PersonDetailsRequestDto>();
        _mockOptions.Setup(opt => opt.Value).Returns(new AccountsServiceApiConfig
        {
            Endpoints = new AccountsServiceEndpoints()
        });
        var client = new AccountServiceClient(new HttpClient(_mockHttpMessageHandler.Object), _mockOptions.Object, _mockLogger.Object);

        // Act and Assert
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.GetPersonDetailsByIds(requestDto));
    }


    [TestMethod]
    public async Task GetOrganisationDetailsById_WhenOrganisationExists_ReturnsExpectedDetails()
    {
        // Arrange
        var id = Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175");
        var expectedDto = _fixture.Create<OrganisationDetailsResponseDto>();
        var responseContent = new StringContent(JsonSerializer.Serialize(expectedDto, JsonSerializerOptions));

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(msg =>
                    msg.Method == HttpMethod.Get &&
                    msg.RequestUri!.ToString().Contains($"organisations/organisation-with-persons")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = responseContent });

        // Act
        var result = await _client.GetOrganisationDetailsById(id);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetNationDetailsById_WithMalformedFormatString_ThrowsFormatException()
    {
        _mockOptions.Setup(opt => opt.Value).Returns(new AccountsServiceApiConfig
        {
            Endpoints = new AccountsServiceEndpoints
            {
                GetOrganisationDetailsById = "organisations/organisation-with-persons/"
            }
        });

        var client = new AccountServiceClient(new HttpClient(_mockHttpMessageHandler.Object), _mockOptions.Object, _mockLogger.Object);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.GetOrganisationDetailsById(Guid.NewGuid()));
    }
}
