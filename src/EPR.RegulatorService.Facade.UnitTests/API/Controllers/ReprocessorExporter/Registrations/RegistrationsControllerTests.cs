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
        var expectedDto = new RegistrationMaterialWasteLicencesDto
        {
            PermitType = "TypeA",
            LicenceNumbers = new[] { "123", "456" },
            CapacityTonne = 100.5m,
            CapacityPeriod = "2025",
            MaximumReprocessingCapacityTonne = 200.0m,
            MaximumReprocessingPeriod = "2026",
            MaterialName = "Plastic"
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

    [TestMethod]
    public async Task GetReprocessingIOByRegistrationMaterialId_ValidRequest_ReturnsExpectedResult()
    {
        // Arrange
        var id = 1;
        var expectedDto = _fixture.Create<RegistrationMaterialReprocessingIODto>();

        _mockRegistrationService
            .Setup(service => service.GetReprocessingIOByRegistrationMaterialId(id))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.GetReprocessingIOByRegistrationMaterialId(id);

        // Assert
        using (new AssertionScope())
        {
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value.Should().BeEquivalentTo(expectedDto);
        }
    }

    [TestMethod]
    public async Task GetReprocessingIOByRegistrationMaterialId_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var id = 1;

        _mockRegistrationService
            .Setup(service => service.GetReprocessingIOByRegistrationMaterialId(id))
            .ThrowsAsync(new Exception("Service error"));

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.GetReprocessingIOByRegistrationMaterialId(id)
        ).Should().ThrowAsync<Exception>()
         .WithMessage("Service error");
    }

    [TestMethod]
    public async Task GetSamplingPlanByRegistrationMaterialId_ValidRequest_ReturnsExpectedResult()
    {
        // Arrange
        var id = 1;
        var expectedDto = _fixture.Create<RegistrationMaterialSamplingPlanDto>();

        _mockRegistrationService
            .Setup(service => service.GetSamplingPlanByRegistrationMaterialId(id))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.GetSamplingPlanByRegistrationMaterialId(id);

        // Assert
        using (new AssertionScope())
        {
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value.Should().BeEquivalentTo(expectedDto);
        }
    }

    [TestMethod]
    public async Task GetSamplingPlanByRegistrationMaterialId_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var id = 1;

        _mockRegistrationService
            .Setup(service => service.GetSamplingPlanByRegistrationMaterialId(id))
            .ThrowsAsync(new Exception("Service error"));

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.GetSamplingPlanByRegistrationMaterialId(id)
        ).Should().ThrowAsync<Exception>()
         .WithMessage("Service error");
    }

    [TestMethod]
    public async Task GetSamplingPlanByRegistrationMaterialId_ServiceReturnsNull_ReturnsOkWithNull()
    {
        // Arrange
        var id = 1;

        _mockRegistrationService
            .Setup(service => service.GetSamplingPlanByRegistrationMaterialId(id))
            .ReturnsAsync((RegistrationMaterialSamplingPlanDto?)null);

        // Act
        var result = await _controller.GetSamplingPlanByRegistrationMaterialId(id);

        // Assert
        using (new AssertionScope())
        {
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value.Should().BeNull();
        }
    }

    [TestMethod]
    public async Task GetSiteAddressByRegistrationId_ShouldReturnOk_WithExpectedResult()
    {
        // Arrange
        var registrationId = 1;
        var expectedDto = _fixture.Create<SiteAddressDetailsDto>();

        _mockRegistrationService
            .Setup(service => service.GetSiteAddressByRegistrationId(registrationId))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.GetSiteAddressByRegistrationId(registrationId);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetSiteAddressByRegistrationId_ShouldThrowException_WhenServiceFails()
    {
        // Arrange
        var registrationId = 1;
        _mockRegistrationService
            .Setup(service => service.GetSiteAddressByRegistrationId(registrationId))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.GetSiteAddressByRegistrationId(registrationId)
        ).Should().ThrowAsync<Exception>()
         .WithMessage("Unexpected error");
    }

    [TestMethod]
    public async Task GetAuthorisedMaterialByRegistrationId_ShouldReturnOk_WithExpectedResult()
    {
        // Arrange
        var registrationId = 2;
        var expectedDto = _fixture.Create<MaterialsAuthorisedOnSiteDto>();

        _mockRegistrationService
            .Setup(service => service.GetAuthorisedMaterialByRegistrationId(registrationId))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.GetAuthorisedMaterialByRegistrationId(registrationId);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetAuthorisedMaterialByRegistrationId_ShouldThrowException_WhenServiceFails()
    {
        // Arrange
        var registrationId = 2;
        _mockRegistrationService
            .Setup(service => service.GetAuthorisedMaterialByRegistrationId(registrationId))
            .ThrowsAsync(new Exception("Service error"));

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.GetAuthorisedMaterialByRegistrationId(registrationId)
        ).Should().ThrowAsync<Exception>()
         .WithMessage("Service error");
    }

    [TestMethod]
    public async Task GetPaymentFeeDetailsByRegistrationMaterialId_ShouldReturnOk_WithExpectedResult()
    {
        // Arrange
        var registrationMaterialId = 2;
        var expectedDto = _fixture.Create<PaymentFeeDetailsDto>();

        _mockRegistrationService
            .Setup(service => service.GetPaymentFeeDetailsByRegistrationMaterialId(registrationMaterialId))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.GetPaymentFeeDetailsByRegistrationMaterialId(registrationMaterialId);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetPaymentFeeDetailsByRegistrationMaterialId_ShouldThrowException_WhenServiceFails()
    {
        // Arrange
        var registrationMaterialId = 2;
        _mockRegistrationService
            .Setup(service => service.GetPaymentFeeDetailsByRegistrationMaterialId(registrationMaterialId))
            .ThrowsAsync(new Exception("Service error"));

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.GetPaymentFeeDetailsByRegistrationMaterialId(registrationMaterialId)
        ).Should().ThrowAsync<Exception>()
         .WithMessage("Service error");
    }
}
