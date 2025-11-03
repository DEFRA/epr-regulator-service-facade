using AutoFixture;
using AutoFixture.AutoMoq;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Services.Application;
using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Services.Regulator
{
    [TestClass]
    public class RegulatorServiceTests
    {
        private ApplicationService _sut;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new();
        private const string BaseAddress = "http://localhost";
        private string PendingApplications = "PendingApplications?userId={0}&currentPage={1}&pageSize={2}&organisationName={3}&applicationType={4}";
        private const string GetOrganisationsApplications = "GetOrganisationsApplications";
        private const string ManageEnrolment = "ManageEnrolment";
        private const string TransferOrganisationNation = "TransferOrganisationNation";
        private const string UserOrganisations = "UserOrganisations";
        private HttpClient _httpClient;
        private readonly NullLogger<ApplicationService> _logger = new();
        private Guid _userId = Guid.NewGuid();
        private Guid _organisationId = Guid.NewGuid();
        private readonly int _currentPage = 1;
        private readonly int _pageSize = 10;
        private readonly string _organisationName = "Org";
        private readonly string _serviceRoleId = "serviceRoleId";
        private string _expectedUrl;

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
        public async Task Should_return_empty_list_when_no_pending_regulators()
        {
            // Arrange
            var template = _configuration.Value.Endpoints.PendingApplications;
            
            var templateWithValues = string.Format(
                CultureInfo.InvariantCulture,
                template,
                Uri.EscapeDataString(_userId.ToString()),
                Uri.EscapeDataString(_currentPage.ToString(CultureInfo.InvariantCulture)),
                Uri.EscapeDataString(_pageSize.ToString(CultureInfo.InvariantCulture)),
                Uri.EscapeDataString(_organisationName),
                Uri.EscapeDataString(_serviceRoleId)
            );

            _expectedUrl = $"{BaseAddress}/{templateWithValues}";

            
            SetupApiCall(0);

            // Act
            var result =
                await _sut.PendingApplications(_userId, _currentPage, _pageSize, _organisationName, _serviceRoleId);

            // Assert
            VerifyApiCall(_expectedUrl, HttpMethod.Get);
            var responseString = await result.Content.ReadAsStringAsync(default);

            var regulators = JsonConvert.DeserializeObject<List<OrganisationEnrolments>>(responseString);
            regulators.Count.Should().Be(0);
        }

        [TestMethod]
        public async Task PendingApplications_Should_Encode_OrganisationName()
        {
            // Arrange
            var organisationName = "E&Z HOLDINGS LTD";
            var applicationType = _serviceRoleId;

            var template = _configuration.Value.Endpoints.PendingApplications;
            var expectedRelativeUrl = string.Format(
                CultureInfo.InvariantCulture,
                template,
                Uri.EscapeDataString(_userId.ToString()),
                Uri.EscapeDataString(_currentPage.ToString(CultureInfo.InvariantCulture)),
                Uri.EscapeDataString(_pageSize.ToString(CultureInfo.InvariantCulture)),
                Uri.EscapeDataString(organisationName),
                Uri.EscapeDataString(applicationType ?? string.Empty)
            );

            var apiResponse = _fixture.CreateMany<OrganisationEnrolments>(1).ToList();
            HttpRequestMessage captured = null!;
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => captured = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(apiResponse))
                })
                .Verifiable();

            // Act
            var result = await _sut.PendingApplications(_userId, _currentPage, _pageSize, organisationName, applicationType);

            // Assert
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());

            captured.Should().NotBeNull();
            var expectedPathAndQuery = "/" + expectedRelativeUrl.TrimStart('/');
            captured.RequestUri!.PathAndQuery.Should().Be(expectedPathAndQuery);
            captured.RequestUri!.OriginalString.Should().Contain("organisationName=E%26Z%20HOLDINGS%20LTD");

            var qs = QueryHelpers.ParseQuery(captured.RequestUri.Query);
            qs["userId"].ToString().Should().Be(_userId.ToString());
            qs["currentPage"].ToString().Should().Be(_currentPage.ToString(CultureInfo.InvariantCulture));
            qs["pageSize"].ToString().Should().Be(_pageSize.ToString(CultureInfo.InvariantCulture));
            qs["organisationName"].ToString().Should().Be(organisationName);
            qs["applicationType"].ToString().Should().Be(applicationType ?? string.Empty);

            var responseString = await result.Content.ReadAsStringAsync(default);
            var regulators = JsonConvert.DeserializeObject<List<OrganisationEnrolments>>(responseString);
            regulators.Count.Should().Be(1);

        }

        [TestMethod]
        public async Task Should_return_pending_regulators_when_available()
        {
            // Arrange

            var template = _configuration.Value.Endpoints.PendingApplications;

            var templateWithValues = string.Format(
                CultureInfo.InvariantCulture,
                template,
                Uri.EscapeDataString(_userId.ToString()),
                Uri.EscapeDataString(_currentPage.ToString(CultureInfo.InvariantCulture)),
                Uri.EscapeDataString(_pageSize.ToString(CultureInfo.InvariantCulture)),
                Uri.EscapeDataString(_organisationName),
                Uri.EscapeDataString(_serviceRoleId)
            );

            _expectedUrl = $"{BaseAddress}/{templateWithValues}";
            SetupApiCall(9);

            // Act
            var result =
                await _sut.PendingApplications(_userId, _currentPage, _pageSize, _organisationName, _serviceRoleId);

            // Assert
            VerifyApiCall(_expectedUrl, HttpMethod.Get);

            var responseString = await result.Content.ReadAsStringAsync(default);

            var regulators = JsonConvert.DeserializeObject<List<OrganisationEnrolments>>(responseString);
            regulators.Count.Should().Be(9);
        }

        [TestMethod]
        public async Task
            When_Get_Enrolments_For_Organisation_Is_Invoked_And_Gets_200_From_Account_Service_Then_Return_Success()
        {
            // Arrange
            var apiResponse = _fixture
                .Build<HttpResponseMessage>()
                .With(x => x.StatusCode, HttpStatusCode.OK)
                .Create();
            var expectedUrl = $"{BaseAddress}/{GetOrganisationsApplications}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();

            // Act
            HttpResponseMessage response = await _sut.GetOrganisationPendingApplications(_userId, _organisationId);
            VerifyApiCall(expectedUrl, HttpMethod.Get);
            // Assert
            response.Should().BeEquivalentTo(apiResponse);
        }

        [TestMethod]
        public async Task
            When_Get_Enrolments_For_Organisation_Is_Invoked_And_Gets_Exception_From_Account_Service_Then_Throw_Exception_OnInternalServerError()
        {
            // Arrange
            var apiResponse = _fixture
                .Build<HttpResponseMessage>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .Create();

            var expectedUrl = $"{BaseAddress}/{GetOrganisationsApplications}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse)
                .Verifiable();

            // Act
            HttpResponseMessage response = await _sut.GetOrganisationPendingApplications(_userId, _organisationId);
            VerifyApiCall(expectedUrl, HttpMethod.Get);

            // Assert
            response.Should().BeEquivalentTo(apiResponse);
        }

        [TestMethod]
        public async Task When_Update_Enrolments_Is_Invoked_And_Gets_200_From_Account_Service_Then_Return_Success()
        {
            // Arrange
            var request = new ManageRegulatorEnrolmentRequest();
            var apiResponse = _fixture
                .Build<HttpResponseMessage>()
                .With(x => x.StatusCode, HttpStatusCode.OK)
                .Create();
            var expectedUrl = $"{BaseAddress}/{ManageEnrolment}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();

            // Act
            HttpResponseMessage response = await _sut.UpdateEnrolment(request);
            VerifyApiCall(expectedUrl, HttpMethod.Post);
            // Assert
            response.Should().BeEquivalentTo(apiResponse);
        }

        [TestMethod]
        public async Task
            When_Update_Enrolment_Is_Invoked_And_Gets_Exception_From_Account_Service_Then_Throw_Exception_OnInternalServerError()
        {
            // Arrange
            var request = new ManageRegulatorEnrolmentRequest();
            var apiResponse = _fixture
                .Build<HttpResponseMessage>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .Create();

            var expectedUrl = $"{BaseAddress}/{ManageEnrolment}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse)
                .Verifiable();

            // Act
            HttpResponseMessage response = await _sut.UpdateEnrolment(request);
            VerifyApiCall(expectedUrl, HttpMethod.Post);

            // Assert
            response.Should().BeEquivalentTo(apiResponse);
        }

        [TestMethod]
        public async Task
            When_Organisation_Nation_Transfer_Is_Invoked_And_Gets_200_From_Account_Service_Then_Return_Success()
        {
            // Arrange
            var request = new OrganisationTransferNationRequest();
            var apiResponse = _fixture
                .Build<HttpResponseMessage>()
                .With(x => x.StatusCode, HttpStatusCode.OK)
                .Create();
            var expectedUrl = $"{BaseAddress}/{TransferOrganisationNation}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();

            // Act
            HttpResponseMessage response = await _sut.TransferOrganisationNation(request);
            VerifyApiCall(expectedUrl, HttpMethod.Post);
            // Assert
            response.Should().BeEquivalentTo(apiResponse);
        }

        [TestMethod]
        public async Task
            When_Organisation_Nation_Transfer_Is_Invoked_And_Gets_Exception_From_Account_Service_Then_Throw_Exception_OnInternalServerError()
        {
            // Arrange
            var request = new OrganisationTransferNationRequest();
            var apiResponse = _fixture
                .Build<HttpResponseMessage>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .Create();

            var expectedUrl = $"{BaseAddress}/{TransferOrganisationNation}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse)
                .Verifiable();

            // Act
            HttpResponseMessage response = await _sut.TransferOrganisationNation(request);
            VerifyApiCall(expectedUrl, HttpMethod.Post);

            // Assert
            response.Should().BeEquivalentTo(apiResponse);
        }

        [TestMethod]
        public async Task When_Get_User_Organisations_Is_Invoked_And_Gets_200_From_Account_Service_Then_Return_Success()
        {
            // Arrange
            var apiResponse = _fixture
                .Build<HttpResponseMessage>()
                .With(x => x.StatusCode, HttpStatusCode.OK)
                .Create();
            var expectedUrl = $"{BaseAddress}/{UserOrganisations}?userId={_userId}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();

            // Act
            HttpResponseMessage response = await _sut.GetUserOrganisations(_userId);
            VerifyApiCall(expectedUrl, HttpMethod.Get);
            // Assert
            response.Should().BeEquivalentTo(apiResponse);
        }

        [TestMethod]
        public async Task
            When_Get_User_Organisations_Is_Invoked_And_Gets_Exception_From_Account_Service_Then_Throw_Exception_OnInternalServerError()
        {
            // Arrange
            var apiResponse = _fixture
                .Build<HttpResponseMessage>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .Create();

            var expectedUrl = $"{BaseAddress}/{UserOrganisations}?userId={_userId}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse)
                .Verifiable();

            // Act
            HttpResponseMessage response = await _sut.GetUserOrganisations(_userId);
            VerifyApiCall(expectedUrl, HttpMethod.Get);

            // Assert
            response.Should().BeEquivalentTo(apiResponse);
        }
        
        private void SetupApiCall(int itemCount)
        {
            var apiResponse = _fixture.CreateMany<OrganisationEnrolments>(itemCount).ToList();

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == _expectedUrl),
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