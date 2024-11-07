using AutoFixture.AutoMoq;
using AutoFixture;
using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.UnitTests.TestHelpers;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;
using System.Xml.Linq;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers;

[TestClass]
public class OrganisationRegistrationSubmissionsControllerTests
{
    private readonly Mock<ILogger<OrganisationRegistrationSubmissionsController>> _loggerMock = new();
    private readonly Mock<ISubmissionService> _submissionsServiceMock = new();
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private OrganisationRegistrationSubmissionsController _sut;
    private readonly Guid _oid = Guid.NewGuid();

    [TestInitialize]
    public void Setup()
    {
        _sut = new OrganisationRegistrationSubmissionsController(
            _submissionsServiceMock.Object,
            _loggerMock.Object);

        _sut.AddDefaultContextWithOid(_oid, "TestAuth");
    }

    [TestMethod]
    [DataRow("SubmissionId", "error")]
    [DataRow("RegistrationStatus", "error")]
    public async Task Should_Return_ValidationProblem_When_ModelState_Is_Invalid(string keyName,string errorMessage)
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
        var request = new RegistrationSubmissionDecisionCreateRequest { 
            OrganisationId = Guid.NewGuid(),
            Status = registrationStatus,
            SubmissionId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.Created)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();

        _submissionsServiceMock.Setup(r => r.CreateSubmissionEvent(
            It.IsAny<Guid>(), It.IsAny<RegistrationSubmissionDecisionEvent>(), It.IsAny<Guid>())).ReturnsAsync(handlerResponse);

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
}
