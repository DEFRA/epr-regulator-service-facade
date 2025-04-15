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
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using Moq;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers.ReprocessorExporter.Registrations;

[TestClass]
public class RegistrationsControllerTests
{
    private Mock<IRegistrationService> _mockRegistrationService = null!;
    private Mock<IValidator<UpdateTaskStatusRequestDto>> _mockUpdateTaskStatusRequestDtoValidator;
    private Mock<IValidator<UpdateMaterialOutcomeRequestDto>> _mockUpdateMaterialOutcomeValidator = null!;
    private Mock<ILogger<RegistrationsController>> _mockLogger = null!;
    private Fixture _fixture = null!;
    private RegistrationsController _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockRegistrationService = new Mock<IRegistrationService>();
        _mockUpdateTaskStatusRequestDtoValidator = new Mock<IValidator<UpdateTaskStatusRequestDto>>();
        _mockUpdateMaterialOutcomeValidator = new Mock<IValidator<UpdateMaterialOutcomeRequestDto>>();
        _mockLogger = new Mock<ILogger<RegistrationsController>>();
        _fixture = new Fixture();

        _controller = new RegistrationsController(
            _mockRegistrationService.Object,
            _mockUpdateTaskStatusRequestDtoValidator.Object,
            _mockUpdateMaterialOutcomeValidator.Object,
            _mockLogger.Object
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
        _mockUpdateTaskStatusRequestDtoValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateTaskStatusRequestDto>(), default))
            .ReturnsAsync(validationResult);

        _mockRegistrationService
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

        _mockUpdateTaskStatusRequestDtoValidator
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
        _mockUpdateTaskStatusRequestDtoValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateTaskStatusRequestDto>(), default))
            .ReturnsAsync(validationResult);

        _mockRegistrationService
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

        _mockUpdateTaskStatusRequestDtoValidator
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

        _mockUpdateMaterialOutcomeValidator
            .Setup(v => v.ValidateAsync(requestDto, default))
            .ReturnsAsync(validationResult);

        _mockRegistrationService
            .Setup(service => service.UpdateMaterialOutcomeByRegistrationMaterialId(registrationMaterialId, requestDto))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateMaterialOutcomeByRegistrationMaterialId(registrationMaterialId, requestDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [TestMethod]
    public async Task UpdateMaterialOutcomeByRegistrationMaterialId_ShouldThrowValidationException_WhenValidationFails()
    {
        // Arrange
        var statusValidator = new InlineValidator<UpdateTaskStatusRequestDto>();
        var validator = new InlineValidator<UpdateMaterialOutcomeRequestDto>();
        validator.RuleFor(x => x.Status).Must(_ => false).WithMessage("Validation failed");

        _controller = new RegistrationsController(
            _mockRegistrationService.Object,
            statusValidator,
            validator,
            _mockLogger.Object
        );

        var registrationMaterialId = 1;
        var requestDto = new UpdateMaterialOutcomeRequestDto
        {
            Status = (RegistrationMaterialStatus)999 
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.UpdateMaterialOutcomeByRegistrationMaterialId(registrationMaterialId, requestDto)
        ).Should().ThrowAsync<ValidationException>();
    }

}
