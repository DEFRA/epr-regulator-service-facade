using AutoFixture;
using EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
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
using System.Net;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers.ReprocessorExporter.Registrations;

[TestClass]
public class RegistrationsControllerTests
{
    private Mock<IRegistrationService> _mockRegistrationService = null!;
    private Mock<IValidator<UpdateRegulatorRegistrationTaskDto>> _mockRegulatorRegistrationValidator = null!;
    private Mock<IValidator<UpdateRegulatorApplicationTaskDto>> _mockRegulatorApplicationValidator = null!;
    private Mock<IValidator<UpdateMaterialOutcomeRequestDto>> _mockUpdateMaterialOutcomeValidator = null!;
    private Mock<ILogger<RegistrationsController>> _mockLogger = null!;
    private Fixture _fixture = null!;
    private RegistrationsController _controller;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockRegistrationService = new Mock<IRegistrationService>();
        _mockRegulatorRegistrationValidator = new Mock<IValidator<UpdateRegulatorRegistrationTaskDto>>();
        _mockRegulatorApplicationValidator = new Mock<IValidator<UpdateRegulatorApplicationTaskDto>>();
        _mockUpdateMaterialOutcomeValidator = new Mock<IValidator<UpdateMaterialOutcomeRequestDto>>();
        _mockLogger = new Mock<ILogger<RegistrationsController>>();
        _fixture = new Fixture();

        _controller = new RegistrationsController(
            _mockRegistrationService.Object,
            _mockRegulatorRegistrationValidator.Object,
            _mockRegulatorApplicationValidator.Object,
            _mockUpdateMaterialOutcomeValidator.Object,
            _mockLogger.Object
        );
    }

    [TestMethod]
    public async Task UpdateRegulatorRegistrationTaskStatus_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = _fixture.Create<UpdateRegulatorRegistrationTaskDto>();
        var validationResult = new ValidationResult();

        _mockRegulatorRegistrationValidator
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);

        _mockRegistrationService
            .Setup(s => s.UpdateRegulatorRegistrationTaskStatus(request))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateRegulatorRegistrationTaskStatus(request);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NoContentResult>();
            ((NoContentResult)result).StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }
    }

    [TestMethod]
    public async Task UpdateRegulatorRegistrationTaskStatus_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var validator = new InlineValidator<UpdateRegulatorRegistrationTaskDto>();
        validator.RuleFor(x => x.RegistrationId).NotEmpty().WithMessage("RegistrationId is required");
        validator.RuleFor(x => x.TaskName).NotEmpty().WithMessage("TaskName is required");
        validator.RuleFor(x => x.Status).IsInEnum().WithMessage("Status is required");
        validator.RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");

        _controller = new RegistrationsController(
            _mockRegistrationService.Object,
            validator,
            _mockRegulatorApplicationValidator.Object,
            _mockUpdateMaterialOutcomeValidator.Object,
            _mockLogger.Object
        );

        var invalidRequest = new UpdateRegulatorRegistrationTaskDto
        {
            RegistrationId = 0,
            TaskName = "",
            Status = 0,
            Comments = "Test",
            UserName = ""
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.UpdateRegulatorRegistrationTaskStatus(invalidRequest)
        ).Should().ThrowAsync<ValidationException>()
         .WithMessage("*is required*");
    }

    [TestMethod]
    public async Task UpdateRegulatorApplicationTaskStatus_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = _fixture.Create<UpdateRegulatorApplicationTaskDto>();
        var validationResult = new ValidationResult();

        _mockRegulatorApplicationValidator
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);

        _mockRegistrationService
            .Setup(s => s.UpdateRegulatorApplicationTaskStatus(request))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateRegulatorApplicationTaskStatus(request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [TestMethod]
    public async Task UpdateRegulatorApplicationTaskStatus_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var validator = new InlineValidator<UpdateRegulatorApplicationTaskDto>();
        validator.RuleFor(x => x.RegistrationMaterialId).NotEmpty().WithMessage("RegistrationMaterialId is required");
        validator.RuleFor(x => x.TaskName).NotEmpty().WithMessage("TaskName is required");
        validator.RuleFor(x => x.Status).IsInEnum().WithMessage("Status is required");
        validator.RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");

        _controller = new RegistrationsController(
            _mockRegistrationService.Object,
            _mockRegulatorRegistrationValidator.Object,
            validator,
            _mockUpdateMaterialOutcomeValidator.Object,
            _mockLogger.Object
        );

        var invalidRequest = new UpdateRegulatorApplicationTaskDto
        {
            RegistrationMaterialId = 0,
            TaskName = "",
            Status = 0,
            Comments = "Testing",
            UserName = ""
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.UpdateRegulatorApplicationTaskStatus(invalidRequest)
        ).Should().ThrowAsync<ValidationException>()
         .WithMessage("*is required*");
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
        var validator = new InlineValidator<UpdateMaterialOutcomeRequestDto>();
        validator.RuleFor(x => x.Status).Must(_ => false).WithMessage("Validation failed");

        _controller = new RegistrationsController(
            _mockRegistrationService.Object,
            _mockRegulatorRegistrationValidator.Object,
            _mockRegulatorApplicationValidator.Object,
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

    [TestMethod]
    public async Task GetWasteLicenceByMaterialId_ValidRequest_ReturnsExpectedResult()
    {
        // Arrange
        var id = 1;
        var expectedDto = new RegistrationMaterialWasteLicenceDto
        {
            PermitType = "TypeA",
            Number = new[] { "123", "456" },
            CapacityTonne = 100.5m,
            Period = "2025",
            MaximumReprocessingCapacityTonne = 200.0m,
            MaximumReprocessingPeriod = "2026",
            Material = "Plastic"
        };

        _mockRegistrationService
            .Setup(service => service.GetWasteLicenceByRegistrationMaterialId(id))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.GetWasteLicenceByRegistrationMaterialId(id);

        // Assert
        using (new AssertionScope())
        {
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value.Should().BeEquivalentTo(expectedDto);
        }
    }
}
