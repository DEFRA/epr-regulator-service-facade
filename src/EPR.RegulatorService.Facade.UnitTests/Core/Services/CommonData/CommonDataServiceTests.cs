using System.Net;
using System.Text.Json;
using AutoFixture;
using AutoFixture.AutoMoq;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData;
using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions.Registrations;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using FluentAssertions;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<CommonDataService> _logger = new Mock<ILogger<CommonDataService>>().Object;
    private const string BaseAddress = "http://localhost";
    private const string GetPoMSubmissions = "GetPoMSubmissions";
    private const string GetOrganisationRegistrationDetails = "GetOrganisationRegistrationDetails";
    private const string GetOrganisationRegistrationSubmissionsSummaries = "GetOrganisationRegistrationSubmissionsSummaries";
    private const string GetPomResubmissionPayCalParameters = "submissions/pom-resubmission-paycal-parameters";
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
                GetOrganisationRegistrationSubmissionDetails = GetOrganisationRegistrationDetails,
                GetOrganisationRegistrationSubmissionsSummaries = GetOrganisationRegistrationSubmissionsSummaries,
                GetPomResubmissionPaycalParameters = GetPomResubmissionPayCalParameters
            }
        });
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri(BaseAddress)
        };

        _sut = new CommonDataService(_httpClient, _configuration, _logger);
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
        var submissionId = Guid.NewGuid();
        _expectedUrl = $"{BaseAddress}/{ _configuration.Value.Endpoints.GetOrganisationRegistrationSubmissionDetails}/{submissionId}";

        var expectedResult = _fixture
            .Build<OrganisationRegistrationDetailsDto>()
            .With(x => x.SubmissionId,submissionId)     // Generate a valid Guid
            .With(x => x.OrganisationId, Guid.NewGuid())   // Another valid Guid
            .With(x => x.SubmittedUserId, Guid.NewGuid())  // Nullable Guid, still valid
            .With(x => x.RegulatorUserId, Guid.NewGuid())  // Another nullable Guid
            .With(x => x.OrganisationType, RandomOrganisationType().ToString())
            .With(x => x.SubmittedDateTime, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"))
            .With(x => x.StatusPendingDate, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"))
            .With(x => x.SubmissionStatus, RandomStatus().ToString())
            .Create();
        
        SetupApiSuccessCall(JsonSerializer.Serialize(expectedResult));

        // Act
        var results = await _sut.GetOrganisationRegistrationSubmissionDetails(submissionId);

        results.Should().BeOfType<RegistrationSubmissionOrganisationDetailsFacadeResponse>();
        Assert.IsNotNull(results);
        Assert.AreEqual(results.SubmissionId, submissionId);
    }

    [TestMethod]
    public async Task Should_Return_Null_When_HTTP_Results_AreEmpty()
    {
        var submissionId = Guid.NewGuid();
        _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetOrganisationRegistrationSubmissionDetails}/{submissionId}";

        SetupNullApiSuccessCall();

        // Act
        var results = await _sut.GetOrganisationRegistrationSubmissionDetails(submissionId);

        results.Should().BeNull();
    }

    [TestMethod]
    public async Task Should_Return_Success_When_Fetching_Summary_List()
    {
        GetOrganisationRegistrationSubmissionsFilter filter = new()
        {
            NationId = 1,
            PageNumber = 1,
            PageSize = 20
        };
        var queryString = $"PageNumber=1&PageSize=20";

        _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetOrganisationRegistrationSubmissionsSummaries}/{filter.NationId}?{queryString}";

        PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse> expectedResponse = new()
        {
            pageSize = 20,
            currentPage = 1,
            items = [],
            totalItems = 0
        };

        SetupApiSuccessCall(JsonSerializer.Serialize(expectedResponse));

        // Act
        var results = await _sut.GetOrganisationRegistrationSubmissionList(filter);

        results.Should().BeOfType<PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>>();
        Assert.IsNotNull(results);
    }


    [TestMethod]
    public async Task Should_Return_Success_With_Deafult_OrganisationRegistrationSubmissionSummary_When_CommonDataApi_Response_Content_IsNull()
    {
        GetOrganisationRegistrationSubmissionsFilter filter = new()
        {
            NationId = 1,
            PageNumber = 1,
            PageSize = 20
        };
        var queryString = $"PageNumber=1&PageSize=20";

        _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetOrganisationRegistrationSubmissionsSummaries}/{filter.NationId}?{queryString}";

        SetupApiSuccessCall();

        // Act
        var results = await _sut.GetOrganisationRegistrationSubmissionList(filter);

        results.Should().BeOfType<PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>>();
        results.Should().NotBeNull();
        results.totalItems.Should().Be(0);  
        results.currentPage.Should().Be(1);
    }

    [TestMethod]
    public async Task Should_Return_Null_When_Api_Response_Is_Empty()
    {
        // Arrange
        var submissionId = Guid.NewGuid();
        
        SetupNullApiSuccessCall();

        // Act
        var result = await _sut.GetPomResubmissionPaycalDetails(submissionId, null);

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task Pom_Resubmission_Should_Construct_Correct_Url_When_ComplianceSchemeId_Is_Not_Provided()
    {
        // Arrange
        var submissionId = Guid.NewGuid();
        _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetPomResubmissionPaycalParameters}/{submissionId}";

        SetupApiSuccessCall("{}");
        
        // Act
        await _sut.GetPomResubmissionPaycalDetails(submissionId, null);

        // Assert
        _httpMessageHandlerMock.Verify(); // Verifies that the expected URL was called
    }

    [TestMethod]
    public async Task Pom_Resubmission_Should_Construct_Correct_Url_When_ComplianceSchemeId_Is_Provided()
    {
        // Arrange
        var submissionId = Guid.NewGuid();
        var complianceSchemeId = Guid.NewGuid();
        _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetPomResubmissionPaycalParameters}/{submissionId}?ComplianceSchemeId={complianceSchemeId}";

        SetupApiSuccessCall("{}");

        // Act
        await _sut.GetPomResubmissionPaycalDetails(submissionId, complianceSchemeId);

        // Assert
        _httpMessageHandlerMock.Verify(); // Verifies that the expected URL was called
    }

    [TestMethod]
    public async Task Pom_Resubmission_Should_Return_Null_When_Response_Is_Empty()
    {
        var submissionId = Guid.NewGuid();
        var complianceSchemeId = Guid.NewGuid();
        _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetPomResubmissionPaycalParameters}/{submissionId}?ComplianceSchemeId={complianceSchemeId}";

        SetupApiSuccessCall("");

        // Act
        var result = await _sut.GetPomResubmissionPaycalDetails(submissionId, complianceSchemeId);

        // Assert
        result.Should().BeNull(); // Verifies that the expected URL was called
    }

    [TestMethod]
    [DataRow(HttpStatusCode.PreconditionFailed, true, false)]
    [DataRow(HttpStatusCode.PreconditionRequired, false, true)]
    public async Task Pom_Resubmission_Should_Return_Correct_Response_For_PreConditionCheck(HttpStatusCode httpStatusCode, bool hasReferenceField, bool hasReference)
    {
        var submissionId = Guid.NewGuid();
        var complianceSchemeId = Guid.NewGuid();
        _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetPomResubmissionPaycalParameters}/{submissionId}?ComplianceSchemeId={complianceSchemeId}";

        var apiResponse = _fixture
            .Build<HttpResponseMessage>()
            .With(x => x.StatusCode, httpStatusCode)
            .Create();

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == _expectedUrl),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(apiResponse).Verifiable();

        // Act
        var result = await _sut.GetPomResubmissionPaycalDetails(submissionId, complianceSchemeId);

        // Assert
        result.Should().NotBeNull();
        result.ReferenceFieldNotAvailable.Should().Be(hasReferenceField);
        result.ReferenceNotAvailable.Should().Be(hasReference);
    }

    [TestMethod]
    public async Task Pom_Resubmission_Should_Return_Null_Response_For_NoContent()
    {
        var submissionId = Guid.NewGuid();
        var complianceSchemeId = Guid.NewGuid();
        _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetPomResubmissionPaycalParameters}/{submissionId}?ComplianceSchemeId={complianceSchemeId}";

        var apiResponse = _fixture
            .Build<HttpResponseMessage>()
            .With(x => x.StatusCode, HttpStatusCode.NoContent)
            .Create();

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == _expectedUrl),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(apiResponse).Verifiable();

        // Act
        var result = await _sut.GetPomResubmissionPaycalDetails(submissionId, complianceSchemeId);

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task Should_Throw_HttpRequestException_when_fetching_registration_submission_details_And_Api_Fails()
    {
        //Arrange
        _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetOrganisationRegistrationSubmissionDetails}";
        SetupApiBadRequestCall();

        // Act
        Assert.ThrowsExceptionAsync<HttpRequestException>(() => _sut.GetOrganisationRegistrationSubmissionDetails(Guid.NewGuid()));
    }

    private static RegistrationSubmissionOrganisationType RandomOrganisationType()
    {
        var values = Enum.GetValues(typeof(RegistrationSubmissionOrganisationType)).Cast<RegistrationSubmissionOrganisationType>().ToArray();
        var random = new Random();
        return values[random.Next(values.Length)];
    }

    private static RegistrationSubmissionStatus RandomStatus()
    {
        var values = Enum.GetValues(typeof(RegistrationSubmissionStatus)).Cast<RegistrationSubmissionStatus>().ToArray();
        var random = new Random();
        return values[random.Next(values.Length)];
    }

    private void SetupApiSuccessCall(string content)
    {
        var apiResponse = _fixture
            .Build<HttpResponseMessage>()
            .With(x => x.StatusCode, HttpStatusCode.OK)
            .With(x => x.Content, new StringContent(content))
            .Create();

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == _expectedUrl),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(apiResponse).Verifiable();
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

    private void SetupNullApiSuccessCall()
    {
        var apiResponse = _fixture
            .Build<HttpResponseMessage>()
            .With(x => x.StatusCode, HttpStatusCode.OK)
            .With(x => x.Content, new StringContent(string.Empty))
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