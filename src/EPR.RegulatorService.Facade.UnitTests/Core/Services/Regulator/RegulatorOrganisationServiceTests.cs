using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Organisations;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions;
using EPR.RegulatorService.Facade.Core.Models.Responses;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Services.Regulator
{ 
    [TestClass]
    public class RegulatorOrganisationServiceTests
    {
        private RegulatorOrganisationService _sut;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new();
        private const string BaseAddress = "http://localhost";
        private const string GetRegulatorUsers = "GetRegulatorUsers";
        private const string GetRegulator = "regulator-organisation?nation=";
        private const string RegulatorInvitation = "regulator-accounts/invite-user";
        private const string RegulatorEnrollment = "accounts-management/enrol-invited-user";
        private const string RegulatorInvitedUser = "regulator-accounts/invited-user?userId={0}&email={1}";
        private HttpClient _httpClient;
        private readonly NullLogger<RegulatorOrganisationService> _logger = new();
        private readonly Guid _userId = Guid.NewGuid();
        private readonly Guid _organisationId = Guid.NewGuid();

        private IOptions<AccountsServiceApiConfig> _configuration = default!;
        
        [TestInitialize]
        public void Setup()
        {
            _configuration = Options.Create(new AccountsServiceApiConfig()
            {
                BaseUrl = BaseAddress,
                Endpoints = new()
                {
                    GetRegulatorUsers = GetRegulatorUsers,
                    GetRegulator = GetRegulator,
                    RegulatorInvitation = RegulatorInvitation,
                    RegulatorEnrollment = RegulatorEnrollment,
                    RegulatorInvitedUser = RegulatorInvitedUser
                }

            });
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };

            _sut = new RegulatorOrganisationService(_httpClient, _logger, _configuration);
        }
        
        [TestMethod]
        public async Task When_Get_Regulator_Users_Is_Invoked_And_Gets_200_From_Account_Service_Then_Return_Success()
        {
            // Arrange
            var apiResponse = _fixture
                .Build<HttpResponseMessage>()
                .With(x => x.StatusCode, HttpStatusCode.OK)
                .Create();
            var expectedUrl = $"{BaseAddress}/{GetRegulatorUsers}?userId={_userId}&organisationId={_organisationId}&getApprovedUsersOnly={true}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();

            // Act
            HttpResponseMessage response = await _sut.GetRegulatorUserList(_userId, _organisationId, true);
            VerifyApiCall(expectedUrl, HttpMethod.Get);
            
            // Assert
            response.Should().BeEquivalentTo(apiResponse);
        }
        
        [TestMethod]
        public async Task When_Get_Regulator_Users_Is_Invoked_And_Gets_Exception_From_Account_Service_Then_Throw_Exception_OnInternalServerError()
        {
            // Arrange
            var apiResponse = _fixture
                .Build<HttpResponseMessage>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .Create();
            var expectedUrl = $"{BaseAddress}/{GetRegulatorUsers}?userId={_userId}&organisationId={_organisationId}&getApprovedUsersOnly={true}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();

            // Act
            HttpResponseMessage response = await _sut.GetRegulatorUserList(_userId, _organisationId, true);
            VerifyApiCall(expectedUrl, HttpMethod.Get);
            
            // Assert
            response.Should().BeEquivalentTo(apiResponse);
        }
        
        [TestMethod]
        public async Task When_Get_Regulator_Organisation_By_Nation_Is_Invoked_And_Gets_200_From_Service_Then_Return_CheckRegulatorOrganisationExistResponseModel()
        {
            // Arrange
            const string nation = "testNation";
            var apiResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new CheckRegulatorOrganisationExistResponseModel()))
            };
            const string expectedUrl = $"{BaseAddress}/{GetRegulator}{nation}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();

            // Act
            var response = await _sut.GetRegulatorOrganisationByNation(nation);
            VerifyApiCall(expectedUrl, HttpMethod.Get);

            // Assert
            response.Should().NotBeNull();
            response.ExternalId.Should().Be(Guid.Empty);
        }
        
        [TestMethod]
        public async Task When_Get_Regulator_Organisation_By_Nation_Is_Invoked_And_Gets_404_From_Service_Then_Return_Null()
        {
            // Arrange
            const string nation = "testNation";
            var apiResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(JsonSerializer.Serialize(new CheckRegulatorOrganisationExistResponseModel()))
            };
            const string expectedUrl = $"{BaseAddress}/{GetRegulator}{nation}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();

            // Act
            var response = await _sut.GetRegulatorOrganisationByNation(nation);
            VerifyApiCall(expectedUrl, HttpMethod.Get);

            // Assert
            response.Should().BeNull();
        }
        
        [TestMethod]
        public async Task When_Get_Regulator_Invite_Is_Invoked_Return_HttpResponseMessage()
        {
            // Arrange
            var request = new AddInviteUserRequest();
            var apiResponse = new HttpResponseMessage(HttpStatusCode.OK);
            const string expectedUrl = $"{BaseAddress}/{RegulatorInvitation}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();

            // Act
            var response = await _sut.RegulatorInvites(request);
            VerifyApiCall(expectedUrl, HttpMethod.Post);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [TestMethod]
        public async Task When_Get_Regulator_Enrolment_Is_Invoked_Return_HttpResponseMessage()
        {
            // Arrange
            var request = new EnrolInvitedUserRequest();
            var apiResponse = new HttpResponseMessage(HttpStatusCode.OK);
            const string expectedUrl = $"{BaseAddress}/{RegulatorEnrollment}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();

            // Act
            var response = await _sut.RegulatorEnrollment(request);
            VerifyApiCall(expectedUrl, HttpMethod.Post);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [TestMethod]
        public async Task When_Get_Regulator_Invited_Is_Invoked_Return_HttpResponseMessage()
        {
            // Arrange
            var id = Guid.NewGuid();
            const string email = "test@test.com";
            
            var apiResponse = new HttpResponseMessage(HttpStatusCode.OK);
            var expectedUrl = $"{BaseAddress}/regulator-accounts/invited-user?userId={id}&email={email}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();

            // Act
            var response = await _sut.RegulatorInvited(id, email);
            VerifyApiCall(expectedUrl, HttpMethod.Get);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [TestMethod]
        public async Task Should_return_empty_list_when_no_results_for_get_producer_users_by_external_id()
        {
            // Arrange
            var expectedUrl = string.Format($"{BaseAddress}/{_configuration.Value.Endpoints.GetUsersByOrganisationExternalId}", _userId, _organisationId);
            
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new List<OrganisationUserOverviewResponseModel>()))
                }).Verifiable();

            // Act
            var result =
                await _sut.GetUsersByOrganisationExternalId(_userId, _organisationId);

            // Assert
            VerifyApiCall(expectedUrl, HttpMethod.Get);
            var responseString = await result.Content.ReadAsStringAsync(default);

            var regulators = JsonConvert.DeserializeObject<List<OrganisationUserOverviewResponseModel>>(responseString);
            regulators.Count.Should().Be(0);
        }

        [TestMethod]
        public async Task AddRemoveApprovedUser_SendValidRequest_ReturnCreatedResult()
        {
            // Arrange
            var apiResponse = new HttpResponseMessage(HttpStatusCode.OK);
            var expectedUrl = string.Format($"{BaseAddress}/{_configuration.Value.Endpoints.AddRemoveApprovedUser}", new AddRemoveApprovedUserRequest());

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();

            // Act
            var response = await _sut.AddRemoveApprovedUser(new AddRemoveApprovedUserRequest());
            VerifyApiCall(expectedUrl, HttpMethod.Post);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task When_Create_Regulator_Organisation_Is_Invoked_And_Gets_BadRequest()
        {
            // Arrange
            var apiResponse = new HttpResponseMessage(HttpStatusCode.OK);
            var expectedUrl = string.Format($"{BaseAddress}/{_configuration.Value.Endpoints.CreateRegulator}", new CreateRegulatorOrganisationResponseModel());

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();

            // Act
            var response = await _sut.CreateRegulatorOrganisation(new CreateRegulatorAccountRequest());
            VerifyApiCall(expectedUrl, HttpMethod.Post);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task When_Create_Regulator_Organisation_Is_Invoked_And_Gets_ExpectedResult()
        {
            // Arrange
            const string nation = "testNation";

            var apiResponse = new HttpResponseMessage(HttpStatusCode.OK);
            apiResponse.Headers.Add("location", "testLocation=" + nation);
            var expectedUrl = string.Format($"{BaseAddress}/{_configuration.Value.Endpoints.CreateRegulator}", new CreateRegulatorOrganisationResponseModel());

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();

            var apiResponse1 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new CheckRegulatorOrganisationExistResponseModel()))
            };

            const string expectedUrl1 = $"{BaseAddress}/{GetRegulator}{nation}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl1),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse1).Verifiable();

            // Act
            var response = await _sut.CreateRegulatorOrganisation(new CreateRegulatorAccountRequest());
            VerifyApiCall(expectedUrl, HttpMethod.Post);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task When_Create_Regulator_Organisation_Is_Invoked_And_CreatedOrganisation_Gets_BadRequest()
        {
            // Arrange
            const string nation = "testNation";

            var apiResponse = new HttpResponseMessage(HttpStatusCode.OK);
            apiResponse.Headers.Add("location", "testLocation=" + nation);
            var expectedUrl = string.Format($"{BaseAddress}/{_configuration.Value.Endpoints.CreateRegulator}", new CreateRegulatorOrganisationResponseModel());

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();

            var apiResponse1 = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
            };

            const string expectedUrl1 = $"{BaseAddress}/{GetRegulator}{nation}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl1),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse1).Verifiable();

            // Act
            var response = await _sut.CreateRegulatorOrganisation(new CreateRegulatorAccountRequest());
            VerifyApiCall(expectedUrl, HttpMethod.Post);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
