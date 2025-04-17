using System;
using System.Threading.Tasks;
using AutoFixture;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using FluentAssertions;
using Moq;

namespace EPR.RegulatorService.Facade.UnitTests.Services.ReprocessorExporter.Registrations;

[TestClass]
public class RegistrationServiceTests
{
    private Mock<IRegistrationServiceClient> _mockClient = null!;
    private RegistrationService _service = null!;
    private Fixture _fixture = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockClient = new Mock<IRegistrationServiceClient>();
        _service = new RegistrationService(_mockClient.Object);
        _fixture = new Fixture();
    }

    [TestMethod]
    public async Task UpdateRegulatorApplicationTaskStatus_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<UpdateRegulatorApplicationTaskDto>();
        _mockClient.Setup(client => client.UpdateRegulatorApplicationTaskStatus(requestDto))
                   .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateRegulatorApplicationTaskStatus(requestDto);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task UpdateRegulatorRegistrationTaskStatus_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<UpdateRegulatorRegistrationTaskDto>();
        _mockClient.Setup(client => client.UpdateRegulatorRegistrationTaskStatus(requestDto))
                   .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateRegulatorRegistrationTaskStatus(requestDto);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task GetRegistrationByRegistrationId_ShouldReturnExpectedResult()
    {
        // Arrange
        var expectedDto = _fixture.Create<RegistrationOverviewDto>();
        _mockClient.Setup(client => client.GetRegistrationByRegistrationId(1))
                   .ReturnsAsync(expectedDto);

        // Act
        var result = await _service.GetRegistrationByRegistrationId(1);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetRegistrationMaterialByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var expectedDto = _fixture.Create<RegistrationMaterialDetailsDto>();
        _mockClient.Setup(client => client.GetRegistrationMaterialByRegistrationMaterialId(1))
                   .ReturnsAsync(expectedDto);

        // Act
        var result = await _service.GetRegistrationMaterialByRegistrationMaterialId(1);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task UpdateMaterialOutcomeByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<UpdateMaterialOutcomeRequestDto>();
        _mockClient.Setup(client => client.UpdateMaterialOutcomeByRegistrationMaterialId(1, requestDto))
                   .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateMaterialOutcomeByRegistrationMaterialId(1, requestDto);

        // Assert
        result.Should().BeTrue();
    }
}


