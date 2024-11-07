using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions.Registrations;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Services.CommonData;

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
            Timeout = 0,
            ServiceRetryCount = 0,
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
    public async Task Should_return_success_when_getting_regulator_pom_summary()
    {
        //Arrange
        var req = new GetPomSubmissionsRequest
        {
            Statuses = "Pending",
            SubmissionYears = "2023",
            SubmissionPeriods = "January to June 2023",
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
                    Comments = "Test",
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
    public async Task Should_return_bad_request_when_getting_regulator_pom_summary()
    {
        //Arrange
        var req = new GetPomSubmissionsRequest
        {
            Statuses = "Pending",
            SubmissionYears = "2023",
            SubmissionPeriods = "January to June 2023",
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
    
    [TestMethod]
    public async Task Should_return_success_when_getting_regulator_registration_summary()
    {
        //Arrange
        var req = new GetRegistrationSubmissionsRequest
        {
            Statuses = "Pending",
            OrganisationName = "test",
            OrganisationReference = "123",
            OrganisationType = "DirectProducer",
            PageNumber = 1,
            PageSize = 20,
            UserId = _userId,
            DecisionsDelta = new RegulatorRegistrationDecision[]
            {
                new()
                {
                    FileId = Guid.NewGuid(),
                    Comments = "Test",
                    Decision = "Approved"
                }
            }
        };
        _expectedUrl =
            $"{BaseAddress}/{_configuration.Value.Endpoints.GetRegistrationSubmissions}";
        SetupApiSuccessCall();

        // Act
        var response = await _sut.GetRegistrationSubmissions(req);

        // Assert
        VerifyApiCall(_expectedUrl, HttpMethod.Post);
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [TestMethod]
    public async Task Should_return_bad_request_when_when_getting_regulator_registration_summary()
    {
        //Arrange
        var req = new GetRegistrationSubmissionsRequest
        {
            Statuses = "Pending",
            OrganisationName = "test",
            OrganisationReference = "123",
            OrganisationType = "DirectProducer",
            PageNumber = 1,
            PageSize = 20,
            UserId = _userId,
            DecisionsDelta = new RegulatorRegistrationDecision[]
            {
                new()
                {
                    FileId = Guid.NewGuid(),
                    Comments = "Test",
                    Decision = "Approved"
                }
            }
        };
        _expectedUrl =
            $"{BaseAddress}/{_configuration.Value.Endpoints.GetRegistrationSubmissions}";

        SetupApiBadRequestCall();

        // Act
        var response = await _sut.GetRegistrationSubmissions(req);

        // Assert
        VerifyApiCall(_expectedUrl, HttpMethod.Post);
        response.IsSuccessStatusCode.Should().BeFalse();
    }

    [TestMethod]
    public async Task Should_return_success_when_fetching_registration_submission_details()
    {
        //Arrange
        _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetRegistrationSubmissionDetails}";
        // Act
        var response = await _sut.GetRegistrationSubmissionDetails(new GetRegistrationSubmissionDetailsRequest());

        // Assert
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