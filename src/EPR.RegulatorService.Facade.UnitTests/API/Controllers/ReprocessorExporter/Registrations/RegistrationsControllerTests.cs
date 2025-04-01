using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers.ReprocessorExporter.Registrations;

[TestClass]
public class RegistrationsControllerTests
{
    private Mock<IRegistrationService> _registrationServiceMock;
    private Mock<IValidator<UpdateTaskStatusRequestDto>> _validatorMock;
    private Mock<ILogger<RegistrationsController>> _loggerMock;
    private RegistrationsController _controller;

    [TestInitialize]
    public void Setup()
    {
        _registrationServiceMock = new Mock<IRegistrationService>();
        _validatorMock = new Mock<IValidator<UpdateTaskStatusRequestDto>>();
        _loggerMock = new Mock<ILogger<RegistrationsController>>();
        _controller = new RegistrationsController(_registrationServiceMock.Object
            , _validatorMock.Object
            , _loggerMock.Object);
    }

    [TestMethod]
    public async Task UpdateRegulatorRegistrationTaskStatus_ReturnsNoContent_WhenValid()
    {
        // Arrange
        var request = new UpdateTaskStatusRequestDto
        {
            Status = RegistrationTaskStatus.Complete,
            Comments = "Valid comments"
        };

        var validationResult = new ValidationResult();
        _validatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);

        _registrationServiceMock
            .Setup(s => s.UpdateRegulatorRegistrationTaskStatus(It.IsAny<int>(), request))
            .ReturnsAsync(true);

        // Act
        var actionResult = await _controller
            .UpdateRegulatorRegistrationTaskStatus(1, request);

        // Assert
        using (new AssertionScope())
        {
            actionResult.Should().BeOfType<NoContentResult>();

            var result = actionResult as NoContentResult;
            result?.Should().NotBeNull();
            result?.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }
    }

    [TestMethod]
    public async Task UpdateRegulatorRegistrationTaskStatus_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = new UpdateTaskStatusRequestDto 
        { 
            Status = RegistrationTaskStatus.Queried 
        };

        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new ("Comments", "Comments is required.")
        });

        _validatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);

        // Act
        var actionResult = await _controller
            .UpdateRegulatorRegistrationTaskStatus(1, request);

        // Assert
        using (new AssertionScope())
        {
            actionResult.Should().BeOfType<BadRequestObjectResult>();

            var result = actionResult as BadRequestObjectResult;
            result?.Should().NotBeNull();
            result?.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }

    [TestMethod]
    public async Task UpdateRegulatorRegistrationTaskStatus_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var request = new UpdateTaskStatusRequestDto 
        { 
            Status = RegistrationTaskStatus.Queried,
            Comments = "Valid comments"
        };

        var validationResult = new ValidationResult();
        _validatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);

        _registrationServiceMock
            .Setup(s => s.UpdateRegulatorRegistrationTaskStatus(It.IsAny<int>(), request))
            .ThrowsAsync(new Exception("Something went wrong"));

        // Act
        var actionResult = await _controller
            .UpdateRegulatorRegistrationTaskStatus(1, request);

        // Assert
        using (new AssertionScope())
        {
            actionResult.Should().BeOfType<StatusCodeResult>();

            var result = actionResult as StatusCodeResult;
            result?.Should().NotBeNull();
            result?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }

    [TestMethod]
    public async Task UpdateRegulatorApplicationTaskStatus_ReturnsNoContent_WhenValid()
    {
        // Arrange
        var request = new UpdateTaskStatusRequestDto
        {
            Status = RegistrationTaskStatus.Complete,
            Comments = "Valid comments"
        };

        var validationResult = new ValidationResult();
        _validatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);

        _registrationServiceMock
            .Setup(s => s.UpdateRegulatorApplicationTaskStatus(It.IsAny<int>(), request))
            .ReturnsAsync(true);

        // Act
        var actionResult = await _controller
            .UpdateRegulatorApplicationTaskStatus(1, request);

        // Assert
        using (new AssertionScope())
        {
            actionResult.Should().BeOfType<NoContentResult>();

            var result = actionResult as NoContentResult;
            result?.Should().NotBeNull();
            result?.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }
    }

    [TestMethod]
    public async Task UpdateRegulatorApplicationTaskStatus_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = new UpdateTaskStatusRequestDto
        {
            Status = RegistrationTaskStatus.Queried
        };

        var validationResult = new ValidationResult(new List<ValidationFailure>
      {
          new ("Comments", "Comments is required.")
      });

        _validatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);

        // Act
        var actionResult = await _controller
            .UpdateRegulatorApplicationTaskStatus(1, request);

        // Assert
        using (new AssertionScope())
        {
            actionResult.Should().BeOfType<BadRequestObjectResult>();

            var result = actionResult as BadRequestObjectResult;
            result?.Should().NotBeNull();
            result?.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }

    [TestMethod]
    public async Task UpdateRegulatorApplicationTaskStatus_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var request = new UpdateTaskStatusRequestDto
        {
            Status = RegistrationTaskStatus.Queried,
            Comments = "Valid comments"
        };

        var validationResult = new ValidationResult();
        _validatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);

        _registrationServiceMock
            .Setup(s => s.UpdateRegulatorApplicationTaskStatus(It.IsAny<int>(), request))
            .ThrowsAsync(new Exception("Something went wrong"));

        // Act
        var actionResult = await _controller
            .UpdateRegulatorApplicationTaskStatus(1, request);

        // Assert
        using (new AssertionScope())
        {
            actionResult.Should().BeOfType<StatusCodeResult>();

            var result = actionResult as StatusCodeResult;
            result?.Should().NotBeNull();
            result?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
