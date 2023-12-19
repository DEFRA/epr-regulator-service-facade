using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace EPR.RegulatorService.Facade.Tests.Core.Services.CommonData;

[TestClass]
public class CommonDataServiceTests
{
    private CommonDataService _sut;
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new();
    private IOptions<CommonDataApiConfig> _configuration = default!;
    private const string BaseAddress = "http://localhost";
    private const string GetPoMSubmissions = "GetPoMSubmissions";
    private HttpClient _httpClient;
    private string _expectedUrl;
    private Guid _userId = Guid.NewGuid();

    [TestInitialize]
    public void Setup()
    {
        _configuration = Options.Create(new CommonDataApiConfig()
        {
            BaseUrl = BaseAddress,
            Endpoints = new()
            {
                GetPoMSubmissions = GetPoMSubmissions
            }
        });
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri(BaseAddress)
        };

        _sut = new CommonDataService(_httpClient, _configuration);
    }

    [TestMethod]
    public async Task Should_return_success_when_fetching_submission_events_last_sync_time()
    {
        //Arrange
        _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetSubmissionEventsLastSyncTime}";
        SetupApiSuccessCall();

        // Act
        var response = await _sut.GetSubmissionLastSyncTime();

        // Assert
        VerifyApiCall(_expectedUrl, HttpMethod.Get);
        response.IsSuccessStatusCode.Should().BeTrue();

    }

    [TestMethod]
    public async Task Should_return_bad_request_when_fetching_submission_events_last_sync_time()
    {
        //Arrange
        _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetSubmissionEventsLastSyncTime}";
        SetupApiBadRequestCall();

        // Act
        var response = await _sut.GetSubmissionLastSyncTime();

        // Assert
        VerifyApiCall(_expectedUrl, HttpMethod.Get);
        response.IsSuccessStatusCode.Should().BeFalse();

    }

    [TestMethod]
    public async Task Should_return_success_when_creating_regulator_pom_decision()
    {
        //Arrange
        var req = new PoMSubmissionsGetRequest
        {
            Statuses = "Pending",
            OrganisationName = "test",
            OrganisationReference = "123",
            OrganisationType = "DirectProducer",
            PageNumber = 1,
            PageSize = 20,
            UserId = _userId,
            DecisionsDelta = new RegulatorPomDecision[]
            {
                new()
                {
                    FileId = Guid.NewGuid(),
                    Decision = "Approved",
                    IsResubmissionRequired = false
                }
            }
        };
        _expectedUrl =
            $"{BaseAddress}/{_configuration.Value.Endpoints.GetPoMSubmissions}";
        SetupApiSuccessCall();

        // Act
        var response = await _sut.GetPoMSubmissions(req);

        // Assert
        VerifyApiCall(_expectedUrl, HttpMethod.Post);
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [TestMethod]
    public async Task Should_return_bad_request_when_creating_regulator_pom_decision()
    {
        //Arrange
        var req = new PoMSubmissionsGetRequest
        {
            Statuses = "Pending",
            OrganisationName = "test",
            OrganisationReference = "123",
            OrganisationType = "DirectProducer",
            PageNumber = 1,
            PageSize = 20,
            UserId = _userId,
            DecisionsDelta = new RegulatorPomDecision[]
            {
                new()
                {
                    FileId = Guid.NewGuid(),
                    Decision = "Approved",
                    IsResubmissionRequired = false
                }
            }
        };
        _expectedUrl =
            $"{BaseAddress}/{_configuration.Value.Endpoints.GetPoMSubmissions}";

        SetupApiBadRequestCall();

        // Act
        var response = await _sut.GetPoMSubmissions(req);

        // Assert
        VerifyApiCall(_expectedUrl, HttpMethod.Post);
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