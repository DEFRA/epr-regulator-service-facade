using System;
using System.Threading.Tasks;
using System.Threading.Tasks;
using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.UnitTests;
using EPR.RegulatorService.Facade.UnitTests.Services;
using EPR.RegulatorService.Facade.UnitTests.Services.ReprocessorExporter.Registrations;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EPR.RegulatorService.Facade.UnitTests.Services.ReprocessorExporter.Registrations;

[TestClass]
public class RegistrationServiceTests
{
    private Mock<IRegistrationServiceClient> _clientMock;
    private RegistrationService _service;

    [TestInitialize]
    public void Setup()
    {
        _clientMock = new Mock<IRegistrationServiceClient>();
        _service = new RegistrationService(_clientMock.Object);
    }

    [TestMethod]
    public async Task UpdateRegulatorRegistrationTaskStatus_CallsClientAndReturnsResult()
    {
        // Arrange
        var dto = new UpdateTaskStatusRequestDto
        {
            Status = RegistrationTaskStatus.Complete
        };

        _clientMock
            .Setup(x => x.UpdateRegulatorRegistrationTaskStatus(1, dto))
            .ReturnsAsync(true);

        // Act
        var result = await _service
            .UpdateRegulatorRegistrationTaskStatus(1, dto);

        // Assert
        result.Should().BeTrue();
        _clientMock.Verify(x => x.UpdateRegulatorRegistrationTaskStatus(1, dto), Times.Once);
    }

    [TestMethod]
    public async Task UpdateRegulatorRegistrationTaskStatus_ThrowsException_WhenClientFails()
    {
        // Arrange
        var dto = new UpdateTaskStatusRequestDto
        {
            Status = RegistrationTaskStatus.Complete
        };

        _clientMock
            .Setup(x => x.UpdateRegulatorRegistrationTaskStatus(1, dto))
            .ThrowsAsync(new HttpRequestException("Something went wrong"));

        // Act
        var act = async () => await _service
        .UpdateRegulatorRegistrationTaskStatus(1, dto);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("Something went wrong");
    }

    [TestMethod]
    public async Task UpdateRegulatorApplicationTaskStatus_CallsClientAndReturnsResult()
    {
        // Arrange
        var dto = new UpdateTaskStatusRequestDto
        {
            Status = RegistrationTaskStatus.Queried
        };

        _clientMock
            .Setup(x => x.UpdateRegulatorApplicationTaskStatus(1, dto))
            .ReturnsAsync(false);

        // Act
        var result = await _service
            .UpdateRegulatorApplicationTaskStatus(1, dto);

        // Assert
        result.Should().BeFalse();
        _clientMock.Verify(x => x.UpdateRegulatorApplicationTaskStatus(1, dto), Times.Once);
    }

    [TestMethod]
    public async Task UpdateRegulatorApplicationTaskStatus_ThrowsException_WhenClientFails()
    {
        // Arrange
        var dto = new UpdateTaskStatusRequestDto
        {
            Status = RegistrationTaskStatus.Complete
        };

        _clientMock.Setup(x => x.UpdateRegulatorApplicationTaskStatus(1, dto))
                   .ThrowsAsync(new HttpRequestException("Something went wrong"));

        // Act
        var act = async () => await _service
        .UpdateRegulatorApplicationTaskStatus(1, dto);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("Something went wrong");
    }
}
