using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;
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
    private Mock<IReprocessorExporterService> _mockRegistrationService = null!;
    private Mock<ILogger<AccreditationsController>> _mockLogger = null!;
    private Fixture _fixture = null!;
    private AccreditationsController _controller = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockRegistrationService = new Mock<IReprocessorExporterService>();
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
        _mockRegistrationService.Setup(service => service.GetRegistrationByIdWithAccreditationsAsync(id, 2025))
                                    .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.GetRegistrationByIdWithAccreditationsAsync(id ,2025);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().BeEquivalentTo(expectedDto);
    }
}
