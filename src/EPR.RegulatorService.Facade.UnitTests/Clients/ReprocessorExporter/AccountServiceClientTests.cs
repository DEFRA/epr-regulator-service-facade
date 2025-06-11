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
                GetNationDetailsById = "regulators/GetNationNameById/{0}",
                GetOrganisationDetailsById = "organisations/organisation-with-persons/{0}"
            }
        });

        _client = new AccountServiceClient(httpClient, _mockOptions.Object, _mockLogger.Object);
        _fixture = new Fixture();
    }

    [TestMethod]
    [DataRow(1, "England")]
    [DataRow(2, "Northern Ireland")]
    [DataRow(3, "Scotland")]
    [DataRow(4, "Wales")]
    [DataRow(99, "Unknown Nation")]
    public async Task GetNationNameById_WhenServiceNotReady_ReturnsHardcodedValue(int nationId, string expected)
    {
        // Act
        var result = await _client.GetNationDetailsById(nationId);

        // Assert
        result.Name.Should().Be(expected);
    }

    [TestMethod]
    public async Task GetOrganisationDetailsById_WhenOrganisationExists_ReturnsExpectedDetails()
    {
        // Arrange
        var id = Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175");
        var expectedDto = _fixture.Create<OrganisationDetailsResponseDto>();
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never };
        var responseContent = new StringContent(JsonSerializer.Serialize(expectedDto, jsonOptions));

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

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => client.GetOrganisationDetailsById(Guid.NewGuid()));
    }
}
