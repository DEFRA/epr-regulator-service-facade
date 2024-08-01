using AutoFixture;
using AutoFixture.AutoMoq;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Helpers;
using EPR.RegulatorService.Facade.Core.Services.Application;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EPR.RegulatorService.Facade.UnitTests.Helpers
{
    [TestClass]
    public class LogHelpersTests
    {
        private ApplicationService _sut;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new();
        private const string BaseAddress = "http://localhost";
        private const string PendingApplications = "PendingApplications";
        private const string GetOrganisationsApplications = "GetOrganisationsApplications";
        private const string ManageEnrolment = "ManageEnrolment";
        private const string TransferOrganisationNation = "TransferOrganisationNation";
        private const string UserOrganisations = "UserOrganisations";
        private HttpClient _httpClient;
        private readonly NullLogger<ApplicationService> _logger = new();

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

            _sut = new ApplicationService(_httpClient, _logger, _configuration);
        }

        [TestMethod]
        public async Task LogInformation()
        {
            // Arrange

            // Act
            LogHelpers.Log(_logger, "Test the logger", Microsoft.Extensions.Logging.LogLevel.Information);

            // Assert
        }

        [TestMethod]
        public async Task LogError()
        {
            // Arrange

            // Act
            LogHelpers.Log(_logger, "Test the logger. Error", Microsoft.Extensions.Logging.LogLevel.Error);

            // Assert
        }
    }
}