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
public class AccountServiceClientTests
{
    private Mock<HttpMessageHandler> _mockHttpMessageHandler = null!;
    private Mock<IOptions<AccountsServiceApiConfig>> _mockOptions = null!;
    private Mock<ILogger<AccountServiceClient>> _mockLogger = null!;
    private AccountServiceClient _client = null!;

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
                GetNationNameById = "regulators/GetNationNameById/{0}"
            }
        });

        _client = new AccountServiceClient(httpClient, _mockOptions.Object, _mockLogger.Object);
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
        var result = await _client.GetNationNameById(nationId);

        // Assert
        result.Should().Be(expected);
    }
}
