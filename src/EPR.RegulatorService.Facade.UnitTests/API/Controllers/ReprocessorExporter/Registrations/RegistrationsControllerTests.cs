﻿using System.Net;
using System.Threading.Tasks;
using EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using Moq;
using FluentAssertions;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentValidation.Results;

namespace EPR.RegulatorService.Facade.UnitTests.Controllers.ReprocessorExporter.Registrations;

[TestClass]
public class RegistrationsControllerTests
{
    private Mock<IRegistrationService> _mockRegistrationService = null!;
    private Mock<IValidator<UpdateMaterialOutcomeRequestDto>> _mockUpdateMaterialOutcomeValidator = null!;
    private Mock<ILogger<RegistrationsController>> _mockLogger = null!;
    private RegistrationsController _controller = null!;
    private Fixture _fixture = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockRegistrationService = new Mock<IRegistrationService>();
        _mockUpdateMaterialOutcomeValidator = new Mock<IValidator<UpdateMaterialOutcomeRequestDto>>();
        _mockLogger = new Mock<ILogger<RegistrationsController>>();

        _controller = new RegistrationsController(
            _mockRegistrationService.Object,
            _mockUpdateMaterialOutcomeValidator.Object,
            _mockLogger.Object
        );

        _fixture = new Fixture();
    }

    [TestMethod]
    public async Task GetRegistrationByRegistrationId_ShouldReturnExpectedResult()
    {
        // Arrange
        var expectedDto = _fixture.Create<RegistrationOverviewDto>();
        var registrationId = 1;
        _mockRegistrationService.Setup(service => service.GetRegistrationByRegistrationId(registrationId))
                                    .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.GetRegistrationByRegistrationId(registrationId);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetRegistrationMaterialByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var expectedDto = _fixture.Create<RegistrationMaterialDetailsDto>();
        var registrationMaterialId = 1;
        _mockRegistrationService.Setup(service => service.GetRegistrationMaterialByRegistrationMaterialId(registrationMaterialId))
                                    .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.GetRegistrationMaterialByRegistrationMaterialId(registrationMaterialId);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task UpdateMaterialOutcomeByRegistrationMaterialId_ShouldReturnNoContent_WhenValidRequest()
    {
        // Arrange
        var registrationMaterialId = 1;
        var requestDto = _fixture.Create<UpdateMaterialOutcomeRequestDto>();

        var validationResult = new ValidationResult(); 
        _mockUpdateMaterialOutcomeValidator.Setup(v => v.ValidateAsync(requestDto, default))
                                            .ReturnsAsync(validationResult);  

        _mockRegistrationService.Setup(service => service.UpdateMaterialOutcomeByRegistrationMaterialId(registrationMaterialId, requestDto))
                                    .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateMaterialOutcomeByRegistrationMaterialId(registrationMaterialId, requestDto);

        // Assert
        result.Should().BeOfType<NoContentResult>(); 
    }

    [TestMethod]
    public async Task UpdateMaterialOutcomeByRegistrationMaterialId_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var registrationMaterialId = 1;
        var requestDto = _fixture.Create<UpdateMaterialOutcomeRequestDto>();

        var validationResult = new ValidationResult();
        validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Field", "Error"));
        _mockUpdateMaterialOutcomeValidator.Setup(v => v.ValidateAsync(requestDto, default))  
                                            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.UpdateMaterialOutcomeByRegistrationMaterialId(registrationMaterialId, requestDto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);  
    }
}
