using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions.Registrations;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.CommonData.ConcreteData;
using EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData;
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
    private const string GetOrganisationRegistrations = "GetOrganisationRegistrations";
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
                GetPoMSubmissions = GetPoMSubmissions,
                GetOrganisationRegistrations = GetOrganisationRegistrations
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
    public async Task GetOrganisationRegistrations_Uses_CommonDataOrganisationRegistrationHandler()
    {
        // Arrange
        var request = new GetOrganisationRegistrationRequest() { PageNumber = 1, PageSize = 1};
        _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetOrganisationRegistrations}";

        SetupApiSuccessCall();

        var result = await _sut.GetOrganisationRegistrations<CommonDataOrganisationRegistrationHandler>(request);

        // Assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetOrganisationRegistrations_Uses_JsonOrganisationRegistrationHandler()
    {
        var request = new GetOrganisationRegistrationRequest() { PageNumber = 1, PageSize = 1 };
        var service = new CommonDataService(_httpClient, _configuration);

        var result = await service.GetOrganisationRegistrations<JsonOrganisationRegistrationHandler>(request);

        Assert.IsNotNull(result); // Ensure the response is valid
    }

    [TestMethod]
    public async Task GetOrganisationRegistrations_ThrowsException_For_UnknownHandler()
    {
        var request = new GetOrganisationRegistrationRequest() { PageNumber = 1, PageSize = 1 };
        var service = new CommonDataService(_httpClient, _configuration);
        
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
        {
            await service.GetOrganisationRegistrations<FakeHandler>(request);
        });
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
public class FakeHandler : IOrganisationRegistrationDataSource
{
    public Task<HttpResponseMessage> GetOrganisationRegistrations(GetOrganisationRegistrationRequest request)
    {
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
    }
}

