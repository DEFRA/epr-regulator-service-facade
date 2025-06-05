using AutoFixture;
using EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers.ReprocessorExporter.Registrations;

[TestClass]
public class RegistrationMaterialsControllerTests
{
    private Mock<IReprocessorExporterService> _mockReprocessorExporterService = null!;
    private Mock<IValidator<UpdateMaterialOutcomeRequestDto>> _mockUpdateMaterialOutcomeValidator = null!;
    private Mock<IValidator<OfflinePaymentRequestDto>> _mockOfflinePaymentRequestValidator = null!;
    private Mock<IValidator<MarkAsDulyMadeRequestDto>> _mockMarkAsDulyMadeRequestValidator = null!;
    private Mock<ILogger<RegistrationMaterialsController>> _mockLogger = null!;
    private Fixture _fixture = null!;
    private RegistrationMaterialsController _controller;

    [TestInitialize]
    public void TestInitialize()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, "testuser@test.com"),
            }, "Test"));

        _mockReprocessorExporterService = new Mock<IReprocessorExporterService>();
        _mockUpdateMaterialOutcomeValidator = new Mock<IValidator<UpdateMaterialOutcomeRequestDto>>();
        _mockOfflinePaymentRequestValidator = new Mock<IValidator<OfflinePaymentRequestDto>>();
        _mockMarkAsDulyMadeRequestValidator = new Mock<IValidator<MarkAsDulyMadeRequestDto>>();
        _mockLogger = new Mock<ILogger<RegistrationMaterialsController>>();
        _fixture = new Fixture();

        _controller = new RegistrationMaterialsController(
            _mockReprocessorExporterService.Object,
            _mockUpdateMaterialOutcomeValidator.Object,
            _mockOfflinePaymentRequestValidator.Object,
            _mockMarkAsDulyMadeRequestValidator.Object,
            _mockLogger.Object
        );

        _controller.ControllerContext = new ControllerContext();
        _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
    }

    [TestMethod]
    public async Task GetRegistrationMaterialByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var expectedDto = _fixture.Create<RegistrationMaterialDetailsDto>();
        var registrationMaterialId = Guid.NewGuid();
        _mockReprocessorExporterService.Setup(service => service.GetRegistrationMaterialByRegistrationMaterialId(registrationMaterialId))
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
        var registrationMaterialId = Guid.NewGuid();
        var requestDto = _fixture.Create<UpdateMaterialOutcomeRequestDto>();
        var validationResult = new ValidationResult();

        _mockUpdateMaterialOutcomeValidator
            .Setup(v => v.ValidateAsync(requestDto, default))
            .ReturnsAsync(validationResult);

        _mockReprocessorExporterService
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

        _controller = new RegistrationMaterialsController(
            _mockReprocessorExporterService.Object,
            validator,
            _mockOfflinePaymentRequestValidator.Object,
            _mockMarkAsDulyMadeRequestValidator.Object,
            _mockLogger.Object
        );

        var registrationMaterialId = Guid.NewGuid();
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
        var id = Guid.NewGuid();
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

        _mockReprocessorExporterService
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
        var id = Guid.NewGuid();
        var expectedDto = _fixture.Create<RegistrationMaterialReprocessingIODto>();

        _mockReprocessorExporterService
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
        var id = Guid.NewGuid();

        _mockReprocessorExporterService
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
        var id = Guid.NewGuid();
        var expectedDto = _fixture.Create<RegistrationMaterialSamplingPlanDto>();

        _mockReprocessorExporterService
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
        var id = Guid.NewGuid();

        _mockReprocessorExporterService
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
        var id = Guid.NewGuid();

        _mockReprocessorExporterService
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
    public async Task GetPaymentFeeDetailsByRegistrationMaterialId_ShouldReturnOk_WithExpectedResult()
    {
        // Arrange
        var registrationMaterialId = Guid.NewGuid();
        var expectedDto = _fixture.Create<PaymentFeeDetailsDto>();

        _mockReprocessorExporterService
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
        var registrationMaterialId = Guid.NewGuid();
        _mockReprocessorExporterService
            .Setup(service => service.GetPaymentFeeDetailsByRegistrationMaterialId(registrationMaterialId))
            .ThrowsAsync(new Exception("Service error"));

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.GetPaymentFeeDetailsByRegistrationMaterialId(registrationMaterialId)
        ).Should().ThrowAsync<Exception>()
         .WithMessage("Service error");
    }

    [TestMethod]
    public async Task SaveOfflinePayment_ShouldReturnNoContent_WhenValidRequest()
    {
        // Arrange
        var requestDto = _fixture.Create<OfflinePaymentRequestDto>();

        _mockOfflinePaymentRequestValidator
            .Setup(v => v.ValidateAsync(requestDto, default))
            .ReturnsAsync(new ValidationResult());

        _mockReprocessorExporterService
            .Setup(s => s.SaveOfflinePayment(It.IsAny<Guid>(), requestDto))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.SaveOfflinePayment(requestDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [TestMethod]
    public async Task SaveOfflinePayment_ShouldThrowValidationException_WhenValidationFails()
    {
        // Arrange
        var validator = new InlineValidator<OfflinePaymentRequestDto>();
        validator.RuleFor(x => x.Amount).Must(_ => false).WithMessage("Invalid");

        _controller = new RegistrationMaterialsController(
            _mockReprocessorExporterService.Object,
            _mockUpdateMaterialOutcomeValidator.Object,
            validator,
            _mockMarkAsDulyMadeRequestValidator.Object,
            _mockLogger.Object
        );

        var requestDto = new OfflinePaymentRequestDto();

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.SaveOfflinePayment(requestDto)
        ).Should().ThrowAsync<ValidationException>();
    }

    [TestMethod]
    public async Task MarkAsDulyMadeByRegistrationMaterialId_ShouldReturnNoContent_WhenValidRequest()
    {
        // Arrange
        var materialId = Guid.NewGuid();
        var requestDto = _fixture.Create<MarkAsDulyMadeRequestDto>();

        _mockMarkAsDulyMadeRequestValidator
            .Setup(v => v.ValidateAsync(requestDto, default))
            .ReturnsAsync(new ValidationResult());

        _mockReprocessorExporterService
            .Setup(s => s.MarkAsDulyMadeByRegistrationMaterialId(materialId, It.IsAny<Guid>(), requestDto))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.MarkAsDulyMadeByRegistrationMaterialId(materialId, requestDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [TestMethod]
    public async Task MarkAsDulyMadeByRegistrationMaterialId_ShouldThrowValidationException_WhenValidationFails()
    {
        // Arrange
        var validator = new InlineValidator<MarkAsDulyMadeRequestDto>();
        validator.RuleFor(x => x.DeterminationDate).Must(_ => false).WithMessage("Invalid");

        _controller = new RegistrationMaterialsController(
            _mockReprocessorExporterService.Object,
            _mockUpdateMaterialOutcomeValidator.Object,
            _mockOfflinePaymentRequestValidator.Object,
            validator,
            _mockLogger.Object
        );

        var requestDto = new MarkAsDulyMadeRequestDto();

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.MarkAsDulyMadeByRegistrationMaterialId(Guid.NewGuid(), requestDto)
        ).Should().ThrowAsync<ValidationException>();
    }
}
