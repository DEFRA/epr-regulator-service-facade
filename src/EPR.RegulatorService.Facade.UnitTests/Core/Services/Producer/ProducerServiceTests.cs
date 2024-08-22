using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models;
using EPR.RegulatorService.Facade.Core.Models.Organisations;
using EPR.RegulatorService.Facade.Core.Services.Producer;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Services.Producer
{
    [TestClass]
    public class ProducerServiceTests
    {
        private ProducerService _sut;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new();
        private const string BaseAddress = "http://localhost";
        private const string PendingApplications = "PendingApplications";
        private const string GetOrganisationsApplications = "GetOrganisationsApplications";
        private const string ManageEnrolment = "ManageEnrolment";
        private const string TransferOrganisationNation = "TransferOrganisationNation";
        private const string UserOrganisations = "UserOrganisations";

        private const string GetProducerUsersByOrganisationExternalId =
            "organisations/producer-users-by-organisation-external-id?userId={0}&externalId={1}";
        private HttpClient _httpClient;
        private readonly NullLogger<ProducerService> _logger = new();
        private readonly Guid _userId = Guid.NewGuid();
        private readonly int _currentPage = 1;
        private readonly int _pageSize = 10;
        private readonly string _organisationName = "Org";
        private readonly Guid _externalId = Guid.NewGuid();
        
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
                    UserOrganisations = UserOrganisations,
                    GetUsersByOrganisationExternalId = GetProducerUsersByOrganisationExternalId
                }
            });
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };

            _sut = new ProducerService(_httpClient, _logger, _configuration);
        }

        
        [TestMethod]
        public async Task Should_return_empty_list_when_no_results_for_organisation_by_searchTerm()
        {
            // Arrange
            var expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetOrganisationsBySearchTerm}";
            SetupApiCall<OrganisationSearchResult>(0, expectedUrl);

            // Act
            var result =
                await _sut.GetOrganisationsBySearchTerm(_userId, _currentPage, _pageSize, _organisationName);

            // Assert
            VerifyApiCall(expectedUrl, HttpMethod.Get);
            var responseString = await result.Content.ReadAsStringAsync();

            var regulators = JsonConvert.DeserializeObject<List<OrganisationSearchResult>>(responseString);
            regulators.Count.Should().Be(0);
        }
        
         
        [TestMethod]
        public async Task RemoveApprovedUser_SendValidRequest_ReturnCreatedResult()
        {
            // Arrange
            var apiResponse = new HttpResponseMessage(HttpStatusCode.OK);
            var expectedUrl = string.Format($"{BaseAddress}/{_configuration.Value.Endpoints.RegulatorRemoveApprovedUser}", new RemoveApprovedUsersRequest());

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();

            // Act
            var response = await _sut.RemoveApprovedUser(new RemoveApprovedUsersRequest());
            VerifyApiCall(expectedUrl, HttpMethod.Post);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task Should_return_empty_list_when_no_results_for_organisation_by_externalId()
        {
            // Arrange
            var expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetOrganisationDetails}";
            SetupApiCall<OrganisationDetails>(0, expectedUrl);

            // Act
            var result =
                await _sut.GetOrganisationDetails(_userId, _externalId);

            // Assert
            VerifyApiCall(expectedUrl, HttpMethod.Get);
            var responseString = await result.Content.ReadAsStringAsync();

            var organisation = JsonConvert.DeserializeObject<List<OrganisationDetails>>(responseString);
            organisation.Count.Should().Be(0);
        }

        private void SetupApiCall<T>(int itemCount, string expectedUrl)
        {
            var apiResponse = _fixture.CreateMany<T>(itemCount).ToList();

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(apiResponse))
                }).Verifiable();
        }

        private void VerifyApiCall(string expectedUrl, HttpMethod method)
        {
            _httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(
                    req => req.Method == method &&
                           req.RequestUri != null &&
                           req.RequestUri.ToString() == expectedUrl),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}