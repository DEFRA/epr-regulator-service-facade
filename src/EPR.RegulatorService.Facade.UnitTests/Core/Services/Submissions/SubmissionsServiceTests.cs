using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Submissions.Events;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Services.Submissions
{
    [TestClass]
    public class SubmissionsServiceTests
    {
        private SubmissionsService _sut;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new();
        private IOptions<SubmissionsApiConfig> _configuration = default!;
        private const string BaseAddress = "http://localhost";
        private const string CreateSubmissionEvent = "CreateSubmissionEvent";
        private const string GetPoMSubmissions = "GetPoMSubmissions";
        private const string GetRegistrationSubmissions = "GetRegistrationSubmissions";
        private HttpClient _httpClient;
        private string _expectedUrl;
        private Guid _userId = Guid.NewGuid();
        private Guid _submissionId = Guid.NewGuid();

        [TestInitialize]
        public void Setup()
        {
            _configuration = Options.Create(new SubmissionsApiConfig()
            {
                BaseUrl = BaseAddress,
                Timeout = 60,
                ServiceRetryCount = 0,
                Endpoints = new()
                {
                    CreateSubmissionEvent = CreateSubmissionEvent,
                    GetPoMSubmissions = GetPoMSubmissions,
                    GetRegistrationSubmissions = GetRegistrationSubmissions
                }
            });
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };

            _sut = new SubmissionsService(_httpClient, _configuration);
        }

        [TestMethod]
        public async Task Should_return_success_when_creating_regulator_pom_decision()
        {
            //Arrange
            var req = _fixture.Create<RegulatorPoMDecisionEvent>();
            _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.CreateSubmissionEvent}";
            SetupApiSuccessCall();

            // Act
            var response = await _sut.CreateSubmissionEvent(_submissionId, req, _userId);

            // Assert
            VerifyApiCall(_expectedUrl, HttpMethod.Post);
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_return_bad_request_when_creating_regulator_pom_decision()
        {
            //Arrange
            var req = _fixture.Create<RegulatorPoMDecisionEvent>();
            _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.CreateSubmissionEvent}";
            SetupApiBadRequestCall();

            // Act
            var response = await _sut.CreateSubmissionEvent(_submissionId, req, _userId);

            // Assert
            VerifyApiCall(_expectedUrl, HttpMethod.Post);
            response.IsSuccessStatusCode.Should().BeFalse();
        }
        
        [TestMethod]
        public async Task Should_return_success_when_fetching_pom_submissions()
        {
            //Arrange
            var lastSyncTime = DateTime.Now;
            _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetPoMSubmissions}?LastSyncTime={lastSyncTime.ToString("yyyy-MM-ddTHH:mm:ss")}";
            SetupApiSuccessCall();

            // Act
            var response = await _sut.GetDeltaPoMSubmissions(lastSyncTime, _userId);

            // Assert
            VerifyApiCall(_expectedUrl, HttpMethod.Get);
            response.IsSuccessStatusCode.Should().BeTrue();
        }
        
        [TestMethod]
        public async Task Should_return_bad_request_when_fetching_pom_submissions()
        {
            //Arrange
            var lastSyncTime = DateTime.Now;
            _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetPoMSubmissions}?LastSyncTime={lastSyncTime.ToString("yyyy-MM-ddTHH:mm:ss")}";
            SetupApiBadRequestCall();

            // Act
            var response = await _sut.GetDeltaPoMSubmissions(lastSyncTime, _userId);

            // Assert
            VerifyApiCall(_expectedUrl, HttpMethod.Get);
            response.IsSuccessStatusCode.Should().BeFalse();
        }
        
        [TestMethod]
        public async Task Should_return_success_when_fetching_registration_submissions()
        {
            //Arrange
            var lastSyncTime = DateTime.Now;
            _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetRegistrationSubmissions}?LastSyncTime={lastSyncTime.ToString("yyyy-MM-ddTHH:mm:ss")}";
            SetupApiSuccessCall();

            // Act
            var response = await _sut.GetDeltaRegistrationSubmissions(lastSyncTime, _userId);

            // Assert
            VerifyApiCall(_expectedUrl, HttpMethod.Get);
            response.IsSuccessStatusCode.Should().BeTrue();
        }
        
        [TestMethod]
        public async Task Should_return_bad_request_when_fetching_registration_submissions()
        {
            //Arrange
            var lastSyncTime = DateTime.Now;
            _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetRegistrationSubmissions}?LastSyncTime={lastSyncTime.ToString("yyyy-MM-ddTHH:mm:ss")}";
            SetupApiBadRequestCall();

            // Act
            var response = await _sut.GetDeltaRegistrationSubmissions(lastSyncTime, _userId);

            // Assert
            VerifyApiCall(_expectedUrl, HttpMethod.Get);
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        private void SetupApiSuccessCall()
        {
            var apiResponse = _fixture
                .Build<HttpResponseMessage>()
                .With(x => x.StatusCode, HttpStatusCode.OK)
                .Create();

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == _expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();
        }

        private void SetupApiBadRequestCall()
        {
            var apiResponse = _fixture
                .Build<HttpResponseMessage>()
                .With(x => x.StatusCode, HttpStatusCode.BadRequest)
                .Create();

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == _expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();
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