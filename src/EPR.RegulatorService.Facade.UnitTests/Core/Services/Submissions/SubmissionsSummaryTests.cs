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

namespace EPR.RegulatorService.Facade.UnitTests.Core.Services.Submissions;

/// <summary>
/// Tests the new API Endpoint in the Submissions API to gather the latest
/// Submission for an Organisation and return it's summary information as required
/// by the regulator journey
/// </summary>
[TestClass]
public class SubmissionSummaryServiceTests
{

    [TestClass]
    public class SubmissionSummaryModelsTests
    {
    }

    [TestClass]
    public class SubmissionServiceRequestsTests
    {
        private SubmissionsService _sut;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new();
        private IOptions<SubmissionsApiConfig> _configuration = default!;
        private const string BaseAddress = "http://localhost";
        private const string GetLatestSubmissionSummary = "GetLatestSubmissionSummary";
        private HttpClient _httpClient;
        private string _expectedUrl;
        private Guid _specificOrgId = Guid.NewGuid();
        private Guid _specificRegulatorId = Guid.NewGuid();

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
                    GetLatestSubmissionSummary = GetLatestSubmissionSummary
                }
            });
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };

            _sut = new SubmissionsService(_httpClient, _configuration);
        }

        [TestMethod]
        public void Should_Return_BadRequest_If_No_Parameter_Values_Specified()
        {
            // Arrange
            var queryParameters = new SubmissionSummaryRequest
            {
                OrganisationID = _specificOrgId,
                UserID = _specificRegulatorId
            };
            _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetLatestSubmissionSummary}";
            SetupApiBadRequestCall();

            // Act
            var response = _sut.GetLatestSubmissionSummary(queryParameters);
            VerifyApiCall(_expectedUrl, HttpMethod.Get);
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [TestMethod]
        public void Should_Return_SummaryResponse_If_Correct_Parameters_Are_Supplied()
        {
            // Arrange
            var queryParameters = new SumissionSummaryRequest
            {
                OrganisationID = _specificOrgId,
                UserID = _specificRegulatorId
            };

            _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetLatestSubmissionSummary}";
            SetupApiSuccessCall(_goodSubmissionSummaryResponse);

            // Act
            var response = _sut.GetLatestSubmissionSummary(queryParameters);
            VerifyApiCall(_expectedUrl, _httpMethod.Get);
            response.IsSuccessStatusCode.Should().BeTrue();
            response.Content.Should().BeType<SubmissionSummary>();
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