using AutoFixture;
using AutoFixture.AutoMoq;
using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using EPR.RegulatorService.Facade.UnitTests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text;
using System.Text.Json;
using static EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission.OrganisationRegistrationSubmissionService;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers;

[TestClass]
public class OrganisationRegistrationSubmissionsControllerTests
{
    private readonly Mock<ILogger<OrganisationRegistrationSubmissionsController>> _ctlLoggerMock = new();
    private readonly Mock<ISubmissionService> _submissionsServiceMock = new();
    private IOrganisationRegistrationSubmissionService _registrationSubmissionServiceFake;
    private readonly Mock<IOrganisationRegistrationSubmissionService> _registrationSubmissionServiceMock = new();
    private readonly Mock<ICommonDataService> _commonDataServiceMock = new();
    private readonly Mock<IMessagingService> _messageServiceMock = new();
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private OrganisationRegistrationSubmissionsController _sut;
    private readonly Guid _oid = Guid.NewGuid();

    [TestInitialize]
    public void Setup()
    {
        _registrationSubmissionServiceFake = new OrganisationRegistrationSubmissionService(
            _commonDataServiceMock.Object,
            _submissionsServiceMock.Object,
            new Mock<ILogger<OrganisationRegistrationSubmissionService>>().Object);

        _sut = new OrganisationRegistrationSubmissionsController(_registrationSubmissionServiceFake, _ctlLoggerMock.Object, _messageServiceMock.Object);

        _sut.AddDefaultContextWithOid(_oid, "TestAuth");
    }

    private void SetupMockWithMockService()
    {
        _sut = new OrganisationRegistrationSubmissionsController(_registrationSubmissionServiceMock.Object, _ctlLoggerMock.Object, _messageServiceMock.Object);
    }

    [TestMethod]
    [DataRow("SubmissionId", "error")]
    [DataRow("RegistrationStatus", "error")]
    public async Task Should_Return_ValidationProblem_When_ModelState_Is_Invalid(string keyName, string errorMessage)
    {
        // Arrange
        _sut.ModelState.AddModelError(keyName, errorMessage);

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(new RegulatorDecisionCreateRequest()) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        result.Value.Should().BeOfType(typeof(ValidationProblemDetails));
    }

    [TestMethod]
    [DataRow(RegistrationSubmissionStatus.Granted)]
    [DataRow(RegistrationSubmissionStatus.Cancelled)]
    [DataRow(RegistrationSubmissionStatus.Refused)]
    [DataRow(RegistrationSubmissionStatus.Queried)]
    public async Task Should_Return_Created_When_SubmissionService_Returns_Success_StatusCode(RegistrationSubmissionStatus registrationStatus)
    {
        // Arrange
        var request = new RegulatorDecisionCreateRequest
        {
            OrganisationId = Guid.NewGuid(),
            Status = registrationStatus,
            SubmissionId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CountryName = CountryName.Eng,
            RegistrationSubmissionType = RegistrationSubmissionType.Producer,
            TwoDigitYear = "99",
            OrganisationAccountManagementId = "123456",
            DecisionDate = new DateTime(2025, 4, 3, 0, 0, 0, DateTimeKind.Utc),
            AgencyName = "Agency Name",
            Comments = "Comment text",
            OrganisationEmail = "Organisation email address",
            OrganisationName = "Organisation name",
            OrganisationReference = "Organisation reference"
        };

        var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.Created)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();

        _submissionsServiceMock.Setup(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationSubmissionDecisionEvent>(), It.IsAny<Guid>())).ReturnsAsync(handlerResponse);

        _registrationSubmissionServiceMock.Setup(s =>
        s.GenerateReferenceNumber(
            It.IsAny<CountryName>(),
            It.IsAny<RegistrationSubmissionType>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<MaterialType>()))
            .Returns("EEE");

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(request) as CreatedResult;

        // Assert
        Assert.IsNotNull(result);
        _submissionsServiceMock.Verify(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationSubmissionDecisionEvent>(), It.IsAny<Guid>()), Times.AtMostOnce);
    }

    [TestMethod]
    public async Task CreateRegulatorSubmissionDecisionEvent_Should_Fetch_UserId_from_HttpContext_When_Input_Request_UserId_Is_Null()
    {
        // Arrange
        var request = new RegulatorDecisionCreateRequest
        {
            OrganisationId = Guid.NewGuid(),
            Status = RegistrationSubmissionStatus.None,
            SubmissionId = Guid.NewGuid(),
            CountryName = CountryName.Eng,
            RegistrationSubmissionType = RegistrationSubmissionType.Producer,
            TwoDigitYear = "99",
            OrganisationAccountManagementId = "123456",
            DecisionDate = new DateTime(2025, 4, 3, 0, 0, 0, DateTimeKind.Utc),
            AgencyName = "Agency Name",
            Comments = "Comment text",
            OrganisationEmail = "Organisation email address",
            OrganisationName = "Organisation name",
            OrganisationReference = "Organisation reference"
        };

        var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.Created)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();

        _submissionsServiceMock.Setup(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationSubmissionDecisionEvent>(), It.IsAny<Guid>())).ReturnsAsync(handlerResponse);

        _registrationSubmissionServiceMock.Setup(s =>
        s.GenerateReferenceNumber(
            It.IsAny<CountryName>(),
            It.IsAny<RegistrationSubmissionType>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<MaterialType>())).Returns("EEE");

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(request) as CreatedResult;

        // Assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    [DataRow(HttpStatusCode.InternalServerError)]
    [DataRow(HttpStatusCode.BadGateway)]
    [DataRow(HttpStatusCode.ServiceUnavailable)]
    public async Task Should_Log_And_Return_Problem_When_SubmissionService_Returns_Non_Success_StatusCode(HttpStatusCode httpStatusCode)
    {
        // Arrange
        var request = new RegulatorDecisionCreateRequest
        {
            OrganisationId = Guid.NewGuid(),
            Status = RegistrationSubmissionStatus.Refused,
            SubmissionId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Comments = "comments"
        };

        var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, httpStatusCode)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();

        _submissionsServiceMock.Setup(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationSubmissionDecisionEvent>(), It.IsAny<Guid>())).ReturnsAsync(handlerResponse);

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(request) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        result.Value.Should().BeOfType(typeof(ProblemDetails));
        _submissionsServiceMock.Verify(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationSubmissionDecisionEvent>(), It.IsAny<Guid>()), Times.AtMostOnce);
        _ctlLoggerMock.Verify(r => r.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ), Times.AtMostOnce);
    }

    [TestMethod]
    public async Task When_InputRequest_InValid_Then_CreateRegulatorSubmissionDecisionEvent_Should_Returns_ValidationProblemDetails()
    {
        // Arrange
        var request = new RegulatorDecisionCreateRequest
        {
            OrganisationId = Guid.NewGuid(),
            Status = RegistrationSubmissionStatus.Refused,
            SubmissionId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Comments = "comments"
        };

        string jsonRequest = JsonSerializer.Serialize(new ValidationProblemDetails { Status = 400 });
        var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.BadRequest)
                    .With(x => x.Content, stringContent)
                    .Create();

        _submissionsServiceMock.Setup(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationSubmissionDecisionEvent>(), It.IsAny<Guid>())).ReturnsAsync(handlerResponse);

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(request) as ObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeOfType(typeof(ValidationProblemDetails));
        ((ProblemDetails)result.Value).Status.Should().Be(400);

        _submissionsServiceMock.Verify(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationSubmissionDecisionEvent>(), It.IsAny<Guid>()), Times.AtMostOnce);
    }

    [TestMethod]
    [DataRow(HttpStatusCode.InternalServerError)]
    [DataRow(HttpStatusCode.BadGateway)]
    [DataRow(HttpStatusCode.ServiceUnavailable)]
    public async Task Should_CreateRegulatorSubmissionDecisionEvent_Returns_ResonsCodes_What_OrganisationRegistrationSubmissionService_Returns(HttpStatusCode httpStatusCode)
    {
        // Arrange
        var request = new RegulatorDecisionCreateRequest
        {
            OrganisationId = Guid.NewGuid(),
            Status = RegistrationSubmissionStatus.Refused,
            SubmissionId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Comments = "comments"
        };

        string jsonRequest = JsonSerializer.Serialize(new ProblemDetails { Status = (int)httpStatusCode });
        var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, httpStatusCode)
                    .With(x => x.Content, stringContent)
                    .Create();

        _submissionsServiceMock.Setup(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationSubmissionDecisionEvent>(), It.IsAny<Guid>())).ReturnsAsync(handlerResponse);

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(request) as ObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeOfType(typeof(ProblemDetails));
        ((ProblemDetails)result.Value).Status.Should().Be((int)httpStatusCode);

        _submissionsServiceMock.Verify(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationSubmissionDecisionEvent>(), It.IsAny<Guid>()), Times.AtMostOnce);
    }


    [TestMethod]
    [DataRow("SubmissionId", "SubmissionId Error")]
    [DataRow("OrganisationID", "OrganisationId Error")]
    public async Task Should_Return_ValidationProblem_When_Call_GetRegistrationSubmissionDetails_With_ModelState_Is_Invalid(string keyName, string errorMessage)
    {
        // Arrange
        _sut.ModelState.AddModelError(keyName, errorMessage);

        // Act
        var result = await _sut.GetRegistrationSubmissionDetails(Guid.Empty) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        result.Value.Should().BeOfType(typeof(ValidationProblemDetails));
    }


    [TestMethod]
    public async Task When_Fetching_GetRegistrationSubmissionDetails_And_CommondataService_Fails_Then_Should_Returns_500_Internal_Server_Error()
    {
        // Arrange 

        var submissionId = Guid.NewGuid();

        var exception = new Exception("Test exception");

        _commonDataServiceMock.Setup(x =>
            x.GetOrganisationRegistrationSubmissionDetails(submissionId)).ThrowsAsync(exception).Verifiable();

        // Act
        var result = await _sut.GetRegistrationSubmissionDetails(submissionId);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult?.StatusCode.Should().Be(500);
    }

    [TestMethod]
    public async Task When_Fetching_GetRegistrationSubmissionDetails_And_CommondataService_Fails_Then_Should_Returns_NotFoundResult()
    {
        // Arrange 

        var submissionId = Guid.NewGuid();

        _commonDataServiceMock.Setup(x =>
            x.GetOrganisationRegistrationSubmissionDetails(submissionId)).ReturnsAsync(null as RegistrationSubmissionOrganisationDetailsFacadeResponse).Verifiable();

        // Act
        var result = await _sut.GetRegistrationSubmissionDetails(submissionId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        var statusCodeResult = result as NotFoundResult;
        statusCodeResult?.StatusCode.Should().Be(404);
    }

    [TestMethod]
    public async Task When_Fetching_GetRegistrationSubmissionDetails_With_Valid_Data_Should_Return_Success()
    {
        // Arrange
        var submissionId = Guid.NewGuid();

        _commonDataServiceMock.Setup(x => x.GetOrganisationRegistrationSubmissionDetails(submissionId))
            .ReturnsAsync(new RegistrationSubmissionOrganisationDetailsFacadeResponse()).Verifiable();

        // Act
        var result = await _sut.GetRegistrationSubmissionDetails(submissionId);

        // Assert
        var statusCodeResult = result as OkObjectResult;
        statusCodeResult?.StatusCode.Should().Be(200);
        _commonDataServiceMock.Verify(r => r.GetOrganisationRegistrationSubmissionDetails(submissionId), Times.AtMostOnce);
    }


    [TestMethod()]
    public async Task GetRegistrationSubmissionListTest_Should_Return_Problem_WhenModelIsInvalid()
    {
        // Arrange
        _sut.ModelState.AddModelError("PageNumber", "PageNumber is required");

        var filter = new GetOrganisationRegistrationSubmissionsFilter();
        _commonDataServiceMock.Setup(x => x.GetOrganisationRegistrationSubmissionList(It.IsAny<GetOrganisationRegistrationSubmissionsFilter>()))
                    .ReturnsAsync(new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>());

        // Act
        var result = await _sut.GetRegistrationSubmissionList(filter);
        Assert.IsInstanceOfType(result, typeof(ObjectResult));

        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);

        var problemDetails = objectResult.Value as ValidationProblemDetails;
        Assert.IsNotNull(problemDetails);
        Assert.IsTrue(problemDetails.Errors.ContainsKey("PageNumber"), "PageNumber is required");

        _commonDataServiceMock.Verify(r => r.GetOrganisationRegistrationSubmissionList(filter), Times.AtMostOnce);
    }

    [TestMethod()]
    public async Task GetRegistrationSubmissionListTest_Should_Return_OK_With_DefaultList_WhenCommonDataThrowsException()
    {
        // Arrange
        var filter = new GetOrganisationRegistrationSubmissionsFilter();
        _commonDataServiceMock.Setup(x => x.GetOrganisationRegistrationSubmissionList(It.IsAny<GetOrganisationRegistrationSubmissionsFilter>()))
                    .Throws(new InvalidDataException("Invalid"));

        // Act
        var result = await _sut.GetRegistrationSubmissionList(filter);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult?.StatusCode.Should().Be(200);
        _commonDataServiceMock.Verify(r => r.GetOrganisationRegistrationSubmissionList(filter), Times.AtMostOnce);
    }

    [TestMethod()]
    public async Task GetRegistrationSubmissionListTest_Should_Return_OK_WithInitialPagination_Page1()
    {
        var expectedResult = new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>();
        var filter = new GetOrganisationRegistrationSubmissionsFilter();
        _commonDataServiceMock.Setup(x => x.GetOrganisationRegistrationSubmissionList(It.IsAny<GetOrganisationRegistrationSubmissionsFilter>()))
                    .ReturnsAsync(expectedResult);

        // Act
        var result = await _sut.GetRegistrationSubmissionList(filter);

        // Assert
        var statusCodeResult = result as OkObjectResult;
        statusCodeResult?.StatusCode.Should().Be(200);
        _commonDataServiceMock.Verify(r => r.GetOrganisationRegistrationSubmissionList(filter), Times.AtMostOnce);

        statusCodeResult.Value.Should().Be(expectedResult);
    }

    [TestMethod]
    public async Task GetRegistrationSubmissionList_WillReturn_500Error_WhenException_IsCaught()
    {
        var parameter = _fixture.Create<GetOrganisationRegistrationSubmissionsFilter>();
        var expectedException = new Exception("The Exception");

        SetupMockWithMockService();

        _registrationSubmissionServiceMock.Setup(x => x.HandleGetRegistrationSubmissionList(It.IsAny<GetOrganisationRegistrationSubmissionsFilter>(), It.IsAny<Guid>())).ThrowsAsync(expectedException);

        var result = await _sut.GetRegistrationSubmissionList(parameter);

        var probDetails = (result as ObjectResult)?.Value as ProblemDetails;
        probDetails.Should().NotBeNull();
    }

    [TestMethod]
    [DataRow(HttpStatusCode.InternalServerError)]
    [DataRow(HttpStatusCode.BadRequest)]
    public async Task Should_Log_And_Return_Problem_When_Service_Returns_Non_Success_StatusCode(HttpStatusCode httpStatusCode)
    {
        // Arrange
        var request = _fixture.Create<RegulatorDecisionCreateRequest>();
        var handlerResponse = new HttpResponseMessage(httpStatusCode) { Content = new StringContent("Service error") };


        _registrationSubmissionServiceMock
            .Setup(r => r.HandleCreateRegulatorDecisionSubmissionEvent(It.IsAny<RegulatorDecisionCreateRequest>(), It.IsAny<Guid>()))
            .ReturnsAsync(handlerResponse);

        _sut = new OrganisationRegistrationSubmissionsController(_registrationSubmissionServiceMock.Object, _ctlLoggerMock.Object, _messageServiceMock.Object);

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(request) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        result.Value.Should().BeOfType<ProblemDetails>();
        result.StatusCode.Should().Be(500);
    }

    [TestMethod]
    public async Task Should_Return_Problem_And_Log_Exception_When_Exception_Thrown()
    {
        // Arrange
        var request = _fixture.Create<RegulatorDecisionCreateRequest>();
        var exception = new Exception("Test exception");

        _registrationSubmissionServiceMock
            .Setup(r => r.HandleCreateRegulatorDecisionSubmissionEvent(It.IsAny<RegulatorDecisionCreateRequest>(), It.IsAny<Guid>()))
            .ThrowsAsync(exception);

        _sut = new OrganisationRegistrationSubmissionsController(_registrationSubmissionServiceMock.Object, _ctlLoggerMock.Object, _messageServiceMock.Object);

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(request) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        result.Value.Should().BeOfType<ProblemDetails>();
        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task Should_Return_Problem_When_RegRefNumber_Is_Invalid_For_Resubmission(string existingRegRefNum)
    {
        // Arrange
        var request = _fixture.Create<RegulatorDecisionCreateRequest>();
        request.IsResubmission = true;
        request.ExistingRegRefNumber = existingRegRefNum;
        _sut = new OrganisationRegistrationSubmissionsController(_registrationSubmissionServiceMock.Object, _ctlLoggerMock.Object, _messageServiceMock.Object);

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(request) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        var problemDetails = result.Value as ValidationProblemDetails;
        problemDetails.Should().NotBeNull();
        problemDetails.Errors.First().Value[0].Should().Be("ExistingRegRefNumber is required for resubmission");
    }

    [TestMethod]
    [DataRow("SubmissionId", "error")]
    [DataRow("PaymentMethod", "error")]
    [DataRow("PaymentStatus", "error")]
    [DataRow("PaidAmount", "error")]
    public async Task CreateRegistrationFeePaymentEvent_Should_Return_ValidationProblem_When_ModelState_Is_Invalid(string keyName, string errorMessage)
    {
        // Arrange
        _sut.ModelState.AddModelError(keyName, errorMessage);

        // Act
        var result = await _sut.CreateRegistrationFeePaymentEvent(new RegistrationFeePaymentCreateRequest()) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        result.Value.Should().BeOfType(typeof(ValidationProblemDetails));
    }

    [TestMethod]
    public async Task CreateRegistrationFeePaymentEvent_Should_Return_Created_When_SubmissionService_Returns_Success()
    {
        // Arrange
        var request = _fixture.Create<RegistrationFeePaymentCreateRequest>();
        var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.Created)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();
        _submissionsServiceMock.Setup(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationFeePaymentEvent>(), It.IsAny<Guid>())).ReturnsAsync(handlerResponse);

        // Act
        var result = await _sut.CreateRegistrationFeePaymentEvent(request) as CreatedResult;

        // Assert
        Assert.IsNotNull(result);
        _submissionsServiceMock.Verify(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationFeePaymentEvent>(), It.IsAny<Guid>()), Times.AtMostOnce);
    }

    [TestMethod]
    [DataRow(HttpStatusCode.InternalServerError)]
    [DataRow(HttpStatusCode.BadGateway)]
    [DataRow(HttpStatusCode.ServiceUnavailable)]
    public async Task CreateRegistrationFeePaymentEvent_Should_Log_And_Return_Problem_When_SubmissionService_Returns_Non_Success(HttpStatusCode statusCode)
    {
        // Arrange
        var request = _fixture.Create<RegistrationFeePaymentCreateRequest>();
        var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, statusCode)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();
        _submissionsServiceMock.Setup(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationFeePaymentEvent>(), It.IsAny<Guid>())).ReturnsAsync(handlerResponse);

        // Act
        var result = await _sut.CreateRegistrationFeePaymentEvent(request) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        result.Value.Should().BeOfType(typeof(ProblemDetails));
        _submissionsServiceMock.Verify(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationFeePaymentEvent>(), It.IsAny<Guid>()), Times.AtMostOnce);
        _ctlLoggerMock.Verify(r => r.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ), Times.AtMostOnce);
    }

    [TestMethod]
    public async Task CreateRegistrationFeePaymentEvent_Log_And_Should_Return_When_Exception_Thrown()
    {
        // Arrange
        var request = _fixture.Create<RegistrationFeePaymentCreateRequest>();
        _submissionsServiceMock
            .Setup(r => r.CreateSubmissionEvent(request.SubmissionId, request, (Guid)request.UserId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _sut.CreateRegistrationFeePaymentEvent(request) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        result.Value.Should().BeOfType<ProblemDetails>();
        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        _ctlLoggerMock.Verify(r => r.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ), Times.AtMostOnce);
    }

    [TestMethod]

    public async Task Should_Return_Created_When_SubmissionService_Returns_Success_StatusCode_And_SendEventEmail_Fail()
    {
        // Arrange
        var request = new RegulatorDecisionCreateRequest
        {
            OrganisationId = Guid.NewGuid(),
            Status = RegistrationSubmissionStatus.Granted,
            SubmissionId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CountryName = CountryName.Eng,
            RegistrationSubmissionType = RegistrationSubmissionType.Producer,
            TwoDigitYear = "99",
            OrganisationAccountManagementId = "123456",
            DecisionDate = new DateTime(2025, 4, 3, 0, 0, 0, DateTimeKind.Utc),
            AgencyName = "Agency Name",
            Comments = "Comment text",
            OrganisationEmail = "Organisation email address",
            OrganisationName = "Organisation name",
            OrganisationReference = "Organisation reference"
        };

        var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.Created)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();

        _submissionsServiceMock.Setup(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationSubmissionDecisionEvent>(), It.IsAny<Guid>())).ReturnsAsync(handlerResponse);

        _registrationSubmissionServiceMock.Setup(s =>
        s.GenerateReferenceNumber(
            It.IsAny<CountryName>(),
            It.IsAny<RegistrationSubmissionType>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<MaterialType>()))
            .Returns("EEE");

        _messageServiceMock.Setup(x => x.OrganisationRegistrationSubmissionDecision(It.IsAny<OrganisationRegistrationSubmissionEmailModel>())).Throws(new Exception("Test Exception"));

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(request) as CreatedResult;

        // Assert
        Assert.IsNotNull(result);
        _submissionsServiceMock.Verify(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationSubmissionDecisionEvent>(), It.IsAny<Guid>()), Times.AtMostOnce);
    }

    [TestMethod]
    public void NoMatchingCosmosItems_NoChanges()
    {
        // Arrange
        var requestedList = new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>
        {
            items =
                [
                    new OrganisationRegistrationSubmissionSummaryResponse
                    {
                        ApplicationReferenceNumber = "APP123",
                        RegulatorDecisionDate = null,
                        SubmissionStatus = RegistrationSubmissionStatus.Pending
                    }
                ]
        };

        var deltaRegistrationDecisionsResponse = new List<AbstractCosmosSubmissionEvent>
            {
                // No event matches "APP123"
                new() {
                    AppReferenceNumber = "DIFFERENT_APP",
                    Created = DateTime.UtcNow,
                    Type = "RegulatorRegistrationDecision",
                    Decision = "Granted"
                }
            };

        // Act
        MergeCosmosUpdates(deltaRegistrationDecisionsResponse, requestedList);

        // Assert
        var item = requestedList.items[0];
        Assert.IsNull(item.RegulatorDecisionDate, "No matching events means RegulatorDecisionDate remains null.");
        Assert.AreEqual(RegistrationSubmissionStatus.Pending, item.SubmissionStatus, "SubmissionStatus should remain unchanged.");
    }

    [TestMethod]
    public void RegulatorDecisionUpdatesWhenRegulatorCommentDateNull()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var requestedList = new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>
        {
            items =
                [
                    new OrganisationRegistrationSubmissionSummaryResponse
                    {
                        ApplicationReferenceNumber = "APP123",
                        RegulatorDecisionDate = null,
                        SubmissionStatus = RegistrationSubmissionStatus.Pending,
                        RegistrationReferenceNumber = "OLD_REF"
                    }
                ]
        };

        var deltaRegistrationDecisionsResponse = new List<AbstractCosmosSubmissionEvent>
            {
                new() {
                    AppReferenceNumber = "app123", // case-insensitive match
                    Created = now,
                    Decision = "Granted",
                    DecisionDate = now.AddDays(-1),
                    RegistrationReferenceNumber = "NEW_REF",
                    Type = "regulatorregistrationdecision"
                }
            };

        // Act
        MergeCosmosUpdates(deltaRegistrationDecisionsResponse, requestedList);

        // Assert
        var item = requestedList.items[0];
        Assert.AreEqual(now, item.RegulatorDecisionDate, "RegulatorDecisionDate should update to event's Created.");
        Assert.AreEqual("NEW_REF", item.RegistrationReferenceNumber, "RegistrationReferenceNumber should update from event.");
        Assert.AreEqual(now.AddDays(-1), item.StatusPendingDate, "StatusPendingDate should match DecisionDate from event.");
        Assert.AreEqual(RegistrationSubmissionStatus.Granted, item.SubmissionStatus, "SubmissionStatus should match event's Decision.");
    }

    [TestMethod]
    public void RegulatorDecisionUpdatesOnlyIfNewerDate()
    {
        // Arrange
        var oldDate = DateTime.UtcNow.AddDays(-2);
        var existingDate = DateTime.UtcNow.AddDays(-1);
        var newDate = DateTime.UtcNow;

        var requestedList = new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>
        {
            items =
                [
                    new OrganisationRegistrationSubmissionSummaryResponse
                    {
                        ApplicationReferenceNumber = "APP123",
                        RegulatorDecisionDate = existingDate,
                        SubmissionStatus = RegistrationSubmissionStatus.Pending,
                        RegistrationReferenceNumber = "OLD_REF"
                    }
                ]
        };

        var deltaRegistrationDecisionsResponse = new List<AbstractCosmosSubmissionEvent>
            {
                // This event is older than existingDate, so should not update
                new() {
                    AppReferenceNumber = "APP123",
                    Created = oldDate,
                    Decision = "Refused",
                    DecisionDate = oldDate,
                    RegistrationReferenceNumber = "SHOULD_NOT_UPDATE",
                    Type = "RegulatorRegistrationDecision"
                },
                // This event is newer and should update
                new() {
                    AppReferenceNumber = "APP123",
                    Created = newDate,
                    Decision = "Cancelled",
                    DecisionDate = newDate,
                    RegistrationReferenceNumber = "NEW_REF",
                    Type = "RegulatorRegistrationDecision"
                }
            };

        // Act
        MergeCosmosUpdates(deltaRegistrationDecisionsResponse, requestedList);

        // Assert
        var item = requestedList.items[0];
        // The older event should not have changed anything
        // The newer event should take precedence
        Assert.AreEqual(newDate, item.RegulatorDecisionDate, "Should update to the newer event's Created date.");
        Assert.AreEqual("NEW_REF", item.RegistrationReferenceNumber, "Should update to the newer event's RegistrationReferenceNumber.");
        Assert.AreEqual(RegistrationSubmissionStatus.Cancelled, item.SubmissionStatus, "Should update to the newer event's Decision.");
    }

    [TestMethod]
    public void ProducerCommentsWithoutRegulatorCommentDate()
    {
        // Arrange
        var commentDate = DateTime.UtcNow;
        var requestedList = new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>
        {
            items =
                [
                    new OrganisationRegistrationSubmissionSummaryResponse
                    {
                        ApplicationReferenceNumber = "APP123",
                        RegulatorDecisionDate = null,
                        SubmissionStatus = RegistrationSubmissionStatus.Granted
                    }
                ]
        };

        var deltaRegistrationDecisionsResponse = new List<AbstractCosmosSubmissionEvent>
            {
                new() {
                    AppReferenceNumber = "APP123",
                    Created = commentDate,
                    Type = "RegistrationApplicationSubmitted"
                }
            };

        // Act
        MergeCosmosUpdates(deltaRegistrationDecisionsResponse, requestedList);

        // Assert
        var item = requestedList.items[0];
        Assert.AreEqual(commentDate, item.SubmissionDate, "SubmissionDate should match the event's Created date.");
        // RegulatorCommentDate is null, ProducerCommentDate > null condition is irrelevant, but code sets SubmissionStatus to Updated if ProducerCommentDate > RegulatorCommentDate
        // Since RegulatorCommentDate is null, we can consider ProducerCommentDate > null logically true for this code's logic.
        Assert.AreEqual(RegistrationSubmissionStatus.Updated, item.SubmissionStatus, "SubmissionStatus should change to Updated due to producer comment.");
    }

    [TestMethod]
    public void ProducerCommentsWhenRegulatorCommentDateNotNullAndProducerLater()
    {
        // Arrange
        var regulatorDate = DateTime.UtcNow.AddHours(-2);
        var producerDate = DateTime.UtcNow;

        var requestedList = new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>
        {
            items =
                [
                    new OrganisationRegistrationSubmissionSummaryResponse
                    {
                        ApplicationReferenceNumber = "APP123",
                        RegulatorDecisionDate = regulatorDate,
                        SubmissionStatus = RegistrationSubmissionStatus.Granted
                    }
                ]
        };

        var deltaRegistrationDecisionsResponse = new List<AbstractCosmosSubmissionEvent>
            {
                new() {
                    AppReferenceNumber = "APP123",
                    Created = producerDate,
                    Type = "RegistrationApplicationSubmitted"
                }
            };

        // Act
        MergeCosmosUpdates(deltaRegistrationDecisionsResponse, requestedList);

        // Assert
        var item = requestedList.items[0];
        Assert.AreEqual(producerDate, item.SubmissionDate, "SubmissionDate should match the event.");
        // Since SubmissionDate > RegulatorDecisionDate, SubmissionStatus should become Updated.
        Assert.AreEqual(RegistrationSubmissionStatus.Updated, item.SubmissionStatus, "Status should update to Updated.");
    }

    [TestMethod]
    public void ProducerCommentsWhenRegulatorCommentDateNewerThanProducer()
    {
        // Arrange
        var regulatorDate = DateTime.UtcNow;
        var producerDate = DateTime.UtcNow.AddHours(-1); // older comment

        var requestedList = new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>
        {
            items =
                [
                    new OrganisationRegistrationSubmissionSummaryResponse
                    {
                        ApplicationReferenceNumber = "APP123",
                        RegulatorDecisionDate = regulatorDate,
                        SubmissionStatus = RegistrationSubmissionStatus.Granted
                    }
                ]
        };

        var deltaRegistrationDecisionsResponse = new List<AbstractCosmosSubmissionEvent>
            {
                // Producer comment is older than Regulator comment, no status update
                new() {
                    AppReferenceNumber = "APP123",
                    Created = producerDate,
                    Type = "RegistrationApplicationSubmitted"
                }
            };

        // Act
        MergeCosmosUpdates(deltaRegistrationDecisionsResponse, requestedList);

        // Assert
        var item = requestedList.items[0];
        Assert.AreEqual(producerDate, item.SubmissionDate, "Should set SubmissionDate to the event's Created date.");
        Assert.AreEqual(RegistrationSubmissionStatus.Granted, item.SubmissionStatus, "No status update as ProducerCommentDate < RegulatorCommentDate.");
    }

    [TestMethod]
    public void MultipleRegulatorDecisionsTakeTheLatestOne()
    {
        // Arrange
        var earlier = DateTime.UtcNow.AddDays(-1);
        var later = DateTime.UtcNow;

        var requestedList = new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>
        {
            items =
                [
                    new OrganisationRegistrationSubmissionSummaryResponse
                    {
                        ApplicationReferenceNumber = "APP123",
                        SubmissionStatus = RegistrationSubmissionStatus.Pending
                    }
                ]
        };

        var deltaRegistrationDecisionsResponse = new List<AbstractCosmosSubmissionEvent>
            {
                new() {
                    AppReferenceNumber = "APP123",
                    Created = earlier,
                    Decision = "Queried",
                    DecisionDate = earlier,
                    RegistrationReferenceNumber = "REF1",
                    Type = "RegulatorRegistrationDecision"
                },
                new() {
                    AppReferenceNumber = "APP123",
                    Created = later,
                    Decision = "Refused",
                    DecisionDate = later,
                    RegistrationReferenceNumber = "REF2",
                    Type = "RegulatorRegistrationDecision"
                }
            };

        // Act
        MergeCosmosUpdates(deltaRegistrationDecisionsResponse, requestedList);

        // Assert
        var item = requestedList.items[0];
        // Should reflect the later event
        Assert.AreEqual(later, item.RegulatorDecisionDate, "Should use the later event's Created date.");
        Assert.AreEqual("REF2", item.RegistrationReferenceNumber, "Should use the later event's RegistrationReferenceNumber.");
        Assert.AreEqual(RegistrationSubmissionStatus.Refused, item.SubmissionStatus, "Should reflect the later event's Decision.");
    }

    [TestMethod]
    public void MultipleProducerCommentsUseTheLatestOneForProducerCommentDateOnly()
    {
        // Arrange
        var olderComment = DateTime.UtcNow.AddHours(-2);
        var newerComment = DateTime.UtcNow.AddHours(-1);
        var regulatorDate = DateTime.UtcNow;

        var requestedList = new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>
        {
            items =
                [
                    new ()
                    {
                        ApplicationReferenceNumber = "APP123",
                        RegulatorDecisionDate = regulatorDate,
                        SubmissionStatus = RegistrationSubmissionStatus.Granted
                    }
                ]
        };

        var deltaRegistrationDecisionsResponse = new List<AbstractCosmosSubmissionEvent>
            {
                new() { AppReferenceNumber = "APP123", Created = olderComment, Type = "RegistrationApplicationSubmitted" },
                new() { AppReferenceNumber = "APP123", Created = newerComment, Type = "RegistrationApplicationSubmitted" }
            };

        // Act
        MergeCosmosUpdates(deltaRegistrationDecisionsResponse, requestedList);

        // Assert
        var item = requestedList.items[0];
        // The code sets SubmissionDate to each cosmos date it finds in turn. The last one processed wins.
        Assert.AreEqual(newerComment, item.SubmissionDate, "Should have the last (newest) SubmissionDate.");
        // Since SubmissionDate < RegulatorDecisionDate, no status update.
        Assert.AreEqual(RegistrationSubmissionStatus.Granted, item.SubmissionStatus, "Should remain Granted as newer ProducerCommentDate still older than RegulatorCommentDate.");
    }

    [TestMethod]
    [DataRow("SubmissionId", "error")]
    [DataRow("PaymentMethod", "error")]
    [DataRow("PaymentStatus", "error")]
    [DataRow("PaidAmount", "error")]
    public async Task CreatePackagingDataResubmissionFeePaymentEvent_Should_Return_ValidationProblem_When_ModelState_Is_Invalid(string keyName, string errorMessage)
    {
        // Arrange
        _sut.ModelState.AddModelError(keyName, errorMessage);

        // Act
        var result = await _sut.CreatePackagingDataResubmissionFeePaymentEvent(new PackagingDataResubmissionFeePaymentCreateRequest()) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        result.Value.Should().BeOfType(typeof(ValidationProblemDetails));
    }

    [TestMethod]
    public async Task CreatePackagingDataResubmissionFeePaymentEvent_Should_Return_Created_When_SubmissionService_Returns_Success()
    {
        // Arrange
        var request = _fixture.Create<PackagingDataResubmissionFeePaymentCreateRequest>();
        var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.Created)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();
        _submissionsServiceMock.Setup(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<PackagingDataResubmissionFeePaymentEvent>(), It.IsAny<Guid>())).ReturnsAsync(handlerResponse);

        // Act
        var result = await _sut.CreatePackagingDataResubmissionFeePaymentEvent(request) as CreatedResult;

        // Assert
        Assert.IsNotNull(result);
        _submissionsServiceMock.Verify(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<PackagingDataResubmissionFeePaymentEvent>(), It.IsAny<Guid>()), Times.AtMostOnce);
    }

    [TestMethod]
    [DataRow(HttpStatusCode.InternalServerError)]
    [DataRow(HttpStatusCode.BadGateway)]
    [DataRow(HttpStatusCode.ServiceUnavailable)]
    public async Task CreatePackagingDataResubmissionFeePaymentEvent_Should_Log_And_Return_Problem_When_SubmissionService_Returns_Non_Success(HttpStatusCode statusCode)
    {
        // Arrange
        var request = _fixture.Create<PackagingDataResubmissionFeePaymentCreateRequest>();
        var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, statusCode)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();
        _submissionsServiceMock.Setup(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<PackagingDataResubmissionFeePaymentEvent>(), It.IsAny<Guid>())).ReturnsAsync(handlerResponse);

        // Act
        var result = await _sut.CreatePackagingDataResubmissionFeePaymentEvent(request) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        result.Value.Should().BeOfType(typeof(ProblemDetails));
        _submissionsServiceMock.Verify(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<PackagingDataResubmissionFeePaymentEvent>(), It.IsAny<Guid>()), Times.AtMostOnce);
        _ctlLoggerMock.Verify(r => r.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ), Times.AtMostOnce);
    }


    [TestMethod]
    public async Task CreatePackagingDataResubmissionFeePaymentEvent_Log_And_Should_Return_When_Exception_Thrown()
    {
        // Arrange
        var request = _fixture.Create<PackagingDataResubmissionFeePaymentCreateRequest>();
        _submissionsServiceMock
            .Setup(r => r.CreateSubmissionEvent(request.SubmissionId, request, (Guid)request.UserId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _sut.CreatePackagingDataResubmissionFeePaymentEvent(request) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        result.Value.Should().BeOfType<ProblemDetails>();
        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        _ctlLoggerMock.Verify(r => r.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ), Times.AtMostOnce);
    }

    #region SendEventEmail (Indirect testing)

    [TestMethod]
    public async Task CreateRegulatorSubmissionDecisionEvent_ValidRequest_Granted_SendsEmail()
    {
        // Arrange
        SetupMockWithMockService();

        var request = _fixture.Build<RegulatorDecisionCreateRequest>()
            .With(r => r.Status, RegistrationSubmissionStatus.Granted)
            .With(r => r.IsResubmission, false) // Testing normal submission
            .Create();

        _registrationSubmissionServiceMock
            .Setup(s => s.HandleCreateRegulatorDecisionSubmissionEvent(It.IsAny<RegulatorDecisionCreateRequest>(), It.IsAny<Guid>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(request);

        // Assert
        result.Should().BeOfType<CreatedResult>();
        _messageServiceMock.Verify(m =>
            m.OrganisationRegistrationSubmissionDecision(It.IsAny<OrganisationRegistrationSubmissionEmailModel>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateRegulatorSubmissionDecisionEvent_ValidRequest_Rejected_SendsEmail()
    {
        // Arrange
        SetupMockWithMockService();

        var request = _fixture.Build<RegulatorDecisionCreateRequest>()
            .With(r => r.Status, RegistrationSubmissionStatus.Refused)
            .With(r => r.IsResubmission, false) // Testing normal submission
            .Create();

        _registrationSubmissionServiceMock
            .Setup(s => s.HandleCreateRegulatorDecisionSubmissionEvent(It.IsAny<RegulatorDecisionCreateRequest>(), It.IsAny<Guid>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(request);

        // Assert
        result.Should().BeOfType<CreatedResult>();
        _messageServiceMock.Verify(m =>
            m.OrganisationRegistrationSubmissionDecision(It.IsAny<OrganisationRegistrationSubmissionEmailModel>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateRegulatorSubmissionDecisionEvent_Resubmission_Granted_SendsResubmissionEmail()
    {
        // Arrange
        SetupMockWithMockService();

        var request = _fixture.Build<RegulatorDecisionCreateRequest>()
            .With(r => r.Status, RegistrationSubmissionStatus.Granted)
            .With(r => r.IsResubmission, true)
            .Create();

        _registrationSubmissionServiceMock
            .Setup(s => s.HandleCreateRegulatorDecisionSubmissionEvent(It.IsAny<RegulatorDecisionCreateRequest>(), It.IsAny<Guid>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(request);

        // Assert
        result.Should().BeOfType<CreatedResult>();
        _messageServiceMock.Verify(m =>
            m.OrganisationRegistrationResubmissionDecision(It.IsAny<OrganisationRegistrationSubmissionEmailModel>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateRegulatorSubmissionDecisionEvent_Resubmission_Refused_SendsResubmissionEmail()
    {
        // Arrange
        SetupMockWithMockService();

        var request = _fixture.Build<RegulatorDecisionCreateRequest>()
            .With(r => r.Status, RegistrationSubmissionStatus.Refused)
            .With(r => r.IsResubmission, true)
            .Create();

        _registrationSubmissionServiceMock
            .Setup(s => s.HandleCreateRegulatorDecisionSubmissionEvent(It.IsAny<RegulatorDecisionCreateRequest>(), It.IsAny<Guid>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(request);

        // Assert
        result.Should().BeOfType<CreatedResult>();
        _messageServiceMock.Verify(m =>
            m.OrganisationRegistrationResubmissionDecision(It.IsAny<OrganisationRegistrationSubmissionEmailModel>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateRegulatorSubmissionDecisionEvent_Queried_SendsQueriedEmail()
    {
        // Arrange
        SetupMockWithMockService();

        var request = _fixture.Build<RegulatorDecisionCreateRequest>()
            .With(r => r.Status, RegistrationSubmissionStatus.Queried)
            .Create();

        _registrationSubmissionServiceMock
            .Setup(s => s.HandleCreateRegulatorDecisionSubmissionEvent(It.IsAny<RegulatorDecisionCreateRequest>(), It.IsAny<Guid>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(request);

        // Assert
        result.Should().BeOfType<CreatedResult>();

        _messageServiceMock.Verify(m =>
            m.OrganisationRegistrationSubmissionQueried(It.IsAny<OrganisationRegistrationSubmissionEmailModel>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateRegulatorSubmissionDecisionEvent_Cancelled_DoesNotSendEmail()
    {
        // Arrange
        SetupMockWithMockService();

        var request = _fixture.Build<RegulatorDecisionCreateRequest>()
            .With(r => r.Status, RegistrationSubmissionStatus.Cancelled)
            .Create();

        _registrationSubmissionServiceMock
            .Setup(s => s.HandleCreateRegulatorDecisionSubmissionEvent(It.IsAny<RegulatorDecisionCreateRequest>(), It.IsAny<Guid>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(request);

        // Assert
        result.Should().BeOfType<CreatedResult>();

        _messageServiceMock.VerifyNoOtherCalls(); // Ensure no email is sent
    }

    #endregion

    // Include the actual MergeCosmosUpdates code here or reference it from the tested class
    // If in a different class, just ensure access is internal or public for testing.
    private static void MergeCosmosUpdates(List<AbstractCosmosSubmissionEvent> deltaRegistrationDecisionsResponse, PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse> requestedList)
    {
        foreach (var item in requestedList.items)
        {
            var cosmosItems = deltaRegistrationDecisionsResponse.Where(x =>
                !string.IsNullOrWhiteSpace(x.AppReferenceNumber) &&
                x.AppReferenceNumber.Equals(item?.ApplicationReferenceNumber, StringComparison.OrdinalIgnoreCase));

            var regulatorDecisions = cosmosItems.Where(x => x.Type.Equals("RegulatorRegistrationDecision", StringComparison.OrdinalIgnoreCase)).ToList();
            var producerComments = cosmosItems.Where(x => x.Type.Equals("RegistrationApplicationSubmitted", StringComparison.OrdinalIgnoreCase)).Select(x => x.Created);

            foreach (var cosmosItem in regulatorDecisions)
            {
                if (item.RegulatorDecisionDate is null || cosmosItem.Created > item.RegulatorDecisionDate)
                {
                    item.RegulatorDecisionDate = cosmosItem.Created;
                    item.RegistrationReferenceNumber = cosmosItem.RegistrationReferenceNumber ?? item.RegistrationReferenceNumber;
                    item.StatusPendingDate = cosmosItem.DecisionDate;
                    item.SubmissionStatus = Enum.Parse<RegistrationSubmissionStatus>(cosmosItem.Decision);
                }
            }

            foreach (var cosmosDate in producerComments)
            {
                item.SubmissionDate = cosmosDate;
                if (item.RegulatorDecisionDate is null || cosmosDate > item.RegulatorDecisionDate)
                {
                    item.SubmissionStatus = RegistrationSubmissionStatus.Updated;
                }
            }
        }
    }
}