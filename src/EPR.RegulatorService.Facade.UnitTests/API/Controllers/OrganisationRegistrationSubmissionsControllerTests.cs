using AutoFixture;
using AutoFixture.AutoMoq;
using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
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

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers;

[TestClass]
public class OrganisationRegistrationSubmissionsControllerTests
{
    private readonly Mock<ILogger<OrganisationRegistrationSubmissionsController>> _ctlLoggerMock = new();
    private readonly Mock<ISubmissionService> _submissionsServiceMock = new();
    private IOrganisationRegistrationSubmissionService _registrationSubmissionServiceFake;
    private readonly Mock<IOrganisationRegistrationSubmissionService> _registrationSubmissionServiceMock = new();
    private readonly Mock<ICommonDataService> _commonDataServiceMock = new();
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private OrganisationRegistrationSubmissionsController _sut;
    private readonly Guid _oid = Guid.NewGuid();

    [TestInitialize]
    public void Setup()
    {
        _registrationSubmissionServiceFake = new OrganisationRegistrationSubmissionService(
            _commonDataServiceMock.Object,
            _submissionsServiceMock.Object);

        _sut = new OrganisationRegistrationSubmissionsController(_registrationSubmissionServiceFake, _ctlLoggerMock.Object);

        _sut.AddDefaultContextWithOid(_oid, "TestAuth");
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
            DecisionDate = new DateTime(2025, 4, 3, 0, 0, 0, DateTimeKind.Utc)
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
        s.GenerateReferenceNumber(It.IsAny<CountryName>(), It.IsAny<RegistrationSubmissionType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MaterialType>())).Returns("EEE");

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(request) as CreatedResult;

        // Assert
        Assert.IsNotNull(result);
        _submissionsServiceMock.Verify(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationSubmissionDecisionEvent>(), It.IsAny<Guid>()), Times.AtMostOnce);
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
    public async Task When_Fetching_GetRegistrationSubmissionDetails_And_CommondataServiice_Fails_Then_Should_Returns_500_Internal_Server_Error()
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
    public async Task When_Fetching_GetRegistrationSubmissionDetails_And_CommondataServiice_Fails_Then_Should_Returns_NotFoundResult()
    {
        // Arrange 

        var submissionId = Guid.NewGuid();

        _commonDataServiceMock.Setup(x =>
            x.GetOrganisationRegistrationSubmissionDetails(submissionId)).ReturnsAsync(null as RegistrationSubmissionOrganisationDetailsResponse).Verifiable();

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
            .ReturnsAsync(new RegistrationSubmissionOrganisationDetailsResponse()).Verifiable();

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
    public async Task GetRegistrationSubmissionListTest_Should_Return_Problem_WhenCommonDataThrowsException()
    {
        // Arrange
        var filter = new GetOrganisationRegistrationSubmissionsFilter();
        _commonDataServiceMock.Setup(x => x.GetOrganisationRegistrationSubmissionList(It.IsAny<GetOrganisationRegistrationSubmissionsFilter>()))
                    .Throws(new InvalidDataException("Invalid"));

        // Act
        var result = await _sut.GetRegistrationSubmissionList(filter);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult?.StatusCode.Should().Be(500);
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

        _sut = new OrganisationRegistrationSubmissionsController(_registrationSubmissionServiceMock.Object, _ctlLoggerMock.Object);

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

        _sut = new OrganisationRegistrationSubmissionsController(_registrationSubmissionServiceMock.Object, _ctlLoggerMock.Object);

        // Act
        var result = await _sut.CreateRegulatorSubmissionDecisionEvent(request) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        result.Value.Should().BeOfType<ProblemDetails>();
        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
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
}