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

    [TestMethod]
    public async Task GetSiteAddressByRegistrationId_ShouldReturnMappedDto()
    {
        // Arrange
        var registrationId = 1;
        var clientResponse = _fixture.Create<RegistrationSiteAddressDto>();
        _mockClient.Setup(client => client.GetSiteAddressByRegistrationId(registrationId))
                   .ReturnsAsync(clientResponse);

        // Act
        var result = await _service.GetSiteAddressByRegistrationId(registrationId);

        // Assert
        result.Should().NotBeNull();
        result.SiteAddress.Should().BeEquivalentTo(clientResponse.SiteAddress);
        result.GridReference.Should().Be(clientResponse.GridReference);
        result.LegalCorrespondenceAddress.Should().BeEquivalentTo(clientResponse.LegalCorrespondenceAddress);
        result.NationName.Should().Be("Endland"); // hardcoded for now
    }

    [TestMethod]
    public async Task GetSiteAddressByRegistrationId_ShouldCallClientExactlyOnce()
    {
        // Arrange
        var registrationId = 5;
        var clientResponse = _fixture.Create<RegistrationSiteAddressDto>();
        _mockClient.Setup(c => c.GetSiteAddressByRegistrationId(registrationId)).ReturnsAsync(clientResponse);

        // Act
        await _service.GetSiteAddressByRegistrationId(registrationId);

        // Assert
        _mockClient.Verify(c => c.GetSiteAddressByRegistrationId(registrationId), Times.Once);
    }

    [TestMethod]
    public async Task GetAuthorisedMaterialByRegistrationId_ShouldReturnExpectedDto()
    {
        // Arrange
        var registrationId = 10;
        var expectedDto = _fixture.Create<MaterialsAuthorisedOnSiteDto>();
        _mockClient.Setup(client => client.GetAuthorisedMaterialByRegistrationId(registrationId))
                   .ReturnsAsync(expectedDto);

        // Act
        var result = await _service.GetAuthorisedMaterialByRegistrationId(registrationId);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetAuthorisedMaterialByRegistrationId_ShouldCallClientExactlyOnce()
    {
        // Arrange
        var registrationId = 42;
        var expectedDto = _fixture.Create<MaterialsAuthorisedOnSiteDto>();
        _mockClient.Setup(client => client.GetAuthorisedMaterialByRegistrationId(registrationId))
                   .ReturnsAsync(expectedDto);

        // Act
        await _service.GetAuthorisedMaterialByRegistrationId(registrationId);

        // Assert
        _mockClient.Verify(client => client.GetAuthorisedMaterialByRegistrationId(registrationId), Times.Once);
    }
}


