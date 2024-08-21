using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Extensions;
using EPR.RegulatorService.Facade.Core.Services.Application;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Policy;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EPR.RegulatorService.Facade.UnitTests.Extensions
{
    [TestClass]
    public class UrlBuilderTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new();
        private const string BaseAddress = "http://localhost";
        private const string PendingApplications = "PendingApplications";
        private const string GetOrganisationsApplications = "GetOrganisationsApplications";
        private const string ManageEnrolment = "ManageEnrolment";
        private const string TransferOrganisationNation = "TransferOrganisationNation";
        private const string UserOrganisations = "UserOrganisations";
        private HttpClient _httpClient;

        private IOptions<AccountsServiceApiConfig> _configuration = default!;

        [TestInitialize]
        public void Setup()
        {
            _configuration = Options.Create(new AccountsServiceApiConfig()
            {
                BaseUrl = BaseAddress,
                Endpoints = new()
                {
                    GetOrganisationsApplications = GetOrganisationsApplications,
                    PendingApplications = PendingApplications,
                    ManageEnrolment = ManageEnrolment,
                    TransferOrganisationNation = TransferOrganisationNation,
                    UserOrganisations = UserOrganisations
                }
            });
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };
        }

        [TestMethod]
        public async Task FormatURL_ValidUrl_ReturnsUrl()
        {
            // Arrange
            string url = $"{BaseAddress}/{_configuration.Value.Endpoints.PendingApplications}";
            
            // Act
            var expectedUrl = UrlBuilderExtention.FormatURL(_httpClient.BaseAddress.ToString(), url);

            // Assert
            expectedUrl.Should().Be(url);
        }

        [TestMethod]
        public async Task FormatURL_IncompleteUrl_ReturnsUrl()
        {
            // Arrange
            string url = $"/{_configuration.Value.Endpoints.PendingApplications}";

            // Act
            var expectedUrl = UrlBuilderExtention.FormatURL(_httpClient.BaseAddress.ToString(), url);

            // Assert
            expectedUrl.Should().Be(url);
        }
    }
}