using AutoFixture.AutoMoq;
using AutoFixture;
using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using Moq;
using EPR.RegulatorService.Facade.UnitTests.TestHelpers;
using Microsoft.Extensions.Logging;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using System.Text.Json;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers;

[TestClass]
public class OrganisationRegistrationSubmissionsControllerTests
{
    private readonly Mock<ILogger<OrganisationRegistrationSubmissionsController>> _loggerMock = new();
    private readonly Mock<ISubmissionService> _submissionsServiceMock = new();
    private readonly Mock<IRegistrationSubmissionService> _registrationSubmissionService = new();
    private readonly Mock<ICommonDataService> _commonDataServiceMock = new();
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private OrganisationRegistrationSubmissionsController _sut;
    private readonly Guid _oid = Guid.NewGuid();

    [TestInitialize]
    public void Setup()
    {
        _sut = new OrganisationRegistrationSubmissionsController(_submissionsServiceMock.Object, _commonDataServiceMock.Object, _loggerMock.Object, _registrationSubmissionService.Object);

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
        var result = await _sut.CreateRegistrationSubmissionDecisionEvent(new RegistrationSubmissionDecisionCreateRequest()) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        result.Value.Should().BeOfType(typeof(ValidationProblemDetails));
    }

    [TestMethod]
    [DataRow(RegistrationStatus.Granted)]
    [DataRow(RegistrationStatus.Cancelled)]
    [DataRow(RegistrationStatus.Refused)]
    [DataRow(RegistrationStatus.Queried)]
    public async Task Should_Return_Created_When_SubmissionService_Returns_Success_StatusCode(RegistrationStatus registrationStatus)
    {
        // Arrange
        var request = new RegistrationSubmissionDecisionCreateRequest
        {
            OrganisationId = Guid.NewGuid(),
            Status = registrationStatus,
            SubmissionId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CountryName = CountryName.Eng,
            RegistrationSubmissionType= RegistrationSubmissionType.Producer,
            TwoDigitYear = "99",
            OrganisationAccountManagementId = "123456"
        };

        var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.Created)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();

        _submissionsServiceMock.Setup(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationSubmissionDecisionEvent>(), It.IsAny<Guid>())).ReturnsAsync(handlerResponse);

        _registrationSubmissionService.Setup(s =>
        s.GenerateReferenceNumber(It.IsAny<CountryName>(), It.IsAny<RegistrationSubmissionType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MaterialType>())).Returns("EEE");

        // Act
        var result = await _sut.CreateRegistrationSubmissionDecisionEvent(request) as CreatedResult;

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
        var request = new RegistrationSubmissionDecisionCreateRequest
        {
            OrganisationId = Guid.NewGuid(),
            Status = RegistrationStatus.Refused,
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
        var result = await _sut.CreateRegistrationSubmissionDecisionEvent(request) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        result.Value.Should().BeOfType(typeof(ProblemDetails));
        _submissionsServiceMock.Verify(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationSubmissionDecisionEvent>(), It.IsAny<Guid>()), Times.AtMostOnce);
        _loggerMock.Verify(r => r.Log(
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

        _commonDataServiceMock.Setup(x => x.GetOrganisationRegistrationSubmissionDetails(submissionId)).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError)).Verifiable();

        // Act
        var result = await _sut.GetRegistrationSubmissionDetails(submissionId);

        // Assert
        result.Should().BeOfType<StatusCodeResult>();
        var statusCodeResult = result as BadRequestResult;
        statusCodeResult?.StatusCode.Should().Be(500);
    }

    [TestMethod]
    public async Task When_Fetching_GetRegistrationSubmissionDetails_With_Valid_Data_Should_Return_Success()
    {
        // Arrange
        var submissionId = Guid.NewGuid();


        _commonDataServiceMock.Setup(x => x.GetOrganisationRegistrationSubmissionDetails(submissionId))
            .ReturnsAsync(new HttpResponseMessage() { Content = new StringContent(JsonSerializer.Serialize(new RegistrationSubmissionOrganisationDetails())) }).Verifiable();

        _sut = new OrganisationRegistrationSubmissionsController(_submissionsServiceMock.Object, _commonDataServiceMock.Object, _loggerMock.Object, _registrationSubmissionService.Object);


        // Act
        var result = await _sut.GetRegistrationSubmissionDetails(submissionId);

        // Assert
        var statusCodeResult = result as OkObjectResult;
        statusCodeResult?.StatusCode.Should().Be(200);
        _commonDataServiceMock.Verify(r => r.GetOrganisationRegistrationSubmissionDetails(submissionId), Times.AtMostOnce);
    }
}