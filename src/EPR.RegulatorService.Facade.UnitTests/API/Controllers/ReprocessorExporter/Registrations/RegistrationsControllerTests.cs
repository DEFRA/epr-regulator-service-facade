using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
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
    private Mock<IValidator<UpdateTaskStatusRequestDto>> _updateTaskStatusRequestDtoValidatorMock;
    private Mock<ILogger<RegistrationsController>> _loggerMock;
    private Fixture _fixture;

    private RegistrationsController _controller;

    [TestInitialize]
    public void Setup()
    {
        _registrationServiceMock = new Mock<IRegistrationService>();
        _updateTaskStatusRequestDtoValidatorMock = new Mock<IValidator<UpdateTaskStatusRequestDto>>();
        _loggerMock = new Mock<ILogger<RegistrationsController>>();
        _fixture = new Fixture();

        _controller = new RegistrationsController(
            _registrationServiceMock.Object,
            _updateTaskStatusRequestDtoValidatorMock.Object,
            _loggerMock.Object
        );
    }

    [TestMethod]
    public async Task UpdateRegulatorRegistrationTaskStatus_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var requestDto = _fixture
            .Build<UpdateTaskStatusRequestDto>()
            .With(p => p.Status, RegistrationTaskStatus.Queried)
            .With(p => p.Comments, "Test comments")
            .Create();

        var validationResult = new ValidationResult();
        _updateTaskStatusRequestDtoValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateTaskStatusRequestDto>(), default))
            .ReturnsAsync(validationResult);

        _registrationServiceMock
            .Setup(s => s.UpdateRegulatorRegistrationTaskStatus(It.IsAny<int>(), It.IsAny<UpdateTaskStatusRequestDto>()))
            .ReturnsAsync(true);

        // Act
        var actionResult = await _controller.UpdateRegulatorRegistrationTaskStatus(id, requestDto);

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
    public async Task UpdateRegulatorRegistrationTaskStatus_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var requestDto = _fixture
            .Build<UpdateTaskStatusRequestDto>()
            .With(p => p.Status, RegistrationTaskStatus.Queried)
            .With(p => p.Comments, string.Empty)
            .Create();

        var validationFailure = new ValidationFailure("Comments", "Comments is required");
        var validationResult = new ValidationResult
        {
            Errors = [validationFailure]
        };

        _updateTaskStatusRequestDtoValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateTaskStatusRequestDto>(), default))
            .ReturnsAsync(validationResult);

        // Act & Assert
        await FluentActions.Invoking(async () =>
        {
            if (validationResult.Errors.Any())
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _controller.UpdateRegulatorRegistrationTaskStatus(id, requestDto);
        })
            .Should().ThrowAsync<ValidationException>();
    }

    [TestMethod]
    public async Task UpdateRegulatorApplicationTaskStatus_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var requestDto = _fixture
            .Build<UpdateTaskStatusRequestDto>()
            .With(p => p.Status, RegistrationTaskStatus.Queried)
            .With(p => p.Comments, "Test comments")
            .Create();

        var validationResult = new ValidationResult();
        _updateTaskStatusRequestDtoValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateTaskStatusRequestDto>(), default))
            .ReturnsAsync(validationResult);

        _registrationServiceMock
            .Setup(s => s.UpdateRegulatorApplicationTaskStatus(It.IsAny<int>(), It.IsAny<UpdateTaskStatusRequestDto>()))
            .ReturnsAsync(true);

        // Act
        var actionResult = await _controller.UpdateRegulatorApplicationTaskStatus(id, requestDto);

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
    public async Task UpdateRegulatorApplicationTaskStatus_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var requestDto = _fixture
            .Build<UpdateTaskStatusRequestDto>()
            .With(p => p.Status, RegistrationTaskStatus.Queried)
            .With(p => p.Comments, string.Empty)
            .Create();

        var validationFailure = new ValidationFailure("Comments", "Comments is required");
        var validationResult = new ValidationResult
        {
            Errors = [validationFailure]
        };

        _updateTaskStatusRequestDtoValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateTaskStatusRequestDto>(), default))
            .ReturnsAsync(validationResult);

        // Act & Assert
        await FluentActions.Invoking(async () =>
        {
            if (validationResult.Errors.Any())
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _controller.UpdateRegulatorApplicationTaskStatus(id, requestDto);
        })
            .Should().ThrowAsync<ValidationException>();
    }
}
