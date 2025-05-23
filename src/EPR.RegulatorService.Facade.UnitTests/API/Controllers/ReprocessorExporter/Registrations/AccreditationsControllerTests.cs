using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers.ReprocessorExporter.Registrations;

[TestClass]
public class AccreditationsControllerTests
{
    private Mock<IRegistrationService> _mockRegistrationService = null!;
    private Mock<ILogger<AccreditationsController>> _mockLogger = null!;
    private Fixture _fixture = null!;
    private AccreditationsController _controller = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockRegistrationService = new Mock<IRegistrationService>();
        _mockLogger = new Mock<ILogger<AccreditationsController>>();
        _fixture = new Fixture();

        _controller = new AccreditationsController(_mockRegistrationService.Object, _mockLogger.Object);
    }

    [TestMethod]
    public async Task GetRegistrationByRegistrationId_ShouldReturnExpectedResult()
    {
        // Arrange
        var expectedDto = _fixture.Create<RegistrationOverviewDto>();
        var id = Guid.NewGuid();
        _mockRegistrationService.Setup(service => service.GetAccreditationsByRegistrationId(id, 2025))
                                    .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.GetAccreditationsByRegistrationId(id ,2025);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetSamplingPlanByAccreditationId_ShouldReturnOKResult()
    {
        //Arrange
        var expectedDto = _fixture.Create<AccreditationSummaryDto>();
        var id = Guid.NewGuid();
        var accreditationId = 5001;
        _mockRegistrationService.Setup(service => service.GetSamplingPlansByAccreditationId(id, accreditationId)).ReturnsAsync(expectedDto);

        //Act
        var result = await _controller.GetSamplingPlansByAccreditationId(id , accreditationId);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetSamplingPlanByAccreditationId_ShouldNotReturnRegistrationOverDto()
    {
        //Arrange
        var actualDto = _fixture.Create<RegistrationOverviewDto>();
        var id = Guid.NewGuid();
        var accreditationId = 1;
        _mockRegistrationService.Setup(service => service.GetSamplingPlansByAccreditationId(id, accreditationId)).Should().NotBeOfType<RegistrationOverviewDto>();

        //Act
        var result = await _controller.GetSamplingPlansByAccreditationId(id, accreditationId);

        var obResult = result as ObjectResult;
        obResult.Should().NotBeNull();
        obResult.Should().NotBeSameAs(actualDto);

        obResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        obResult.Value.Should().NotBeSameAs(actualDto);
    }
}
