using AutoFixture;
using EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Accreditations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Security.Claims;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers.ReprocessorExporter.Registrations;

[TestClass]
public class AccreditationsControllerTests
{
    private Mock<IReprocessorExporterService> _mockReprocessorExporterService = null!;
    private Mock<ILogger<AccreditationsController>> _mockLogger = null!;
    private Fixture _fixture = null!;
    private AccreditationsController _controller = null!;

    private Mock<IAccreditationService> _mockAccreditationService = null!;
    private Mock<IValidator<AccreditationMarkAsDulyMadeRequestDto>> _mockMarkAsDulyMadeRequestValidator = null!;
    private Mock<IValidator<UpdateAccreditationMaterialTaskStatusDto>> _mockRegulatorApplicationValidator = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "testuser@test.com"),
        }, "Test"));

        _mockReprocessorExporterService = new Mock<IReprocessorExporterService>();
        _mockAccreditationService = new Mock<IAccreditationService>();
        _mockLogger = new Mock<ILogger<AccreditationsController>>();
        _mockMarkAsDulyMadeRequestValidator = new Mock<IValidator<AccreditationMarkAsDulyMadeRequestDto>>();
        _mockRegulatorApplicationValidator = new Mock<IValidator<UpdateAccreditationMaterialTaskStatusDto>>();

        _fixture = new Fixture();
        _controller = new AccreditationsController(_mockReprocessorExporterService.Object,
                                                _mockAccreditationService.Object,
                                                _mockMarkAsDulyMadeRequestValidator.Object,
                                                _mockRegulatorApplicationValidator.Object,
                                                _mockLogger.Object);

        _controller.ControllerContext = new ControllerContext();
        _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
    }

    [TestMethod]
    public async Task GetRegistrationByRegistrationId_ShouldReturnExpectedResult()
    {
        // Arrange
        var expectedDto = _fixture.Create<RegistrationOverviewDto>();
        var id = Guid.NewGuid();
        _mockReprocessorExporterService.Setup(service => service.GetRegistrationByIdWithAccreditationsAsync(id, 2025))
                                    .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.GetRegistrationByIdWithAccreditationsAsync(id, 2025);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetPaymentFeeDetailsByAccreditationMaterialId_ShouldReturnOk_WithExpectedResult()
    {
        // Arrange
        Guid accreditationMaterialId = Guid.NewGuid();

        var expectedDto = _fixture.Create<AccreditationPaymentFeeDetailsDto>();

        _mockAccreditationService.Setup(service => service.GetPaymentFeeDetailsByAccreditationMaterialId(accreditationMaterialId))
                                 .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.GetPaymentFeeDetailsByAccreditationMaterialId(accreditationMaterialId);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetPaymentFeeDetailsByAccreditationMaterialId_ShouldThrowException_WhenServiceFails()
    {
        // Arrange
        var accreditationMaterialId = Guid.NewGuid();
        _mockAccreditationService.Setup(service => service.GetPaymentFeeDetailsByAccreditationMaterialId(accreditationMaterialId))
                                .ThrowsAsync(new Exception("Service error"));

        // Act & Assert
        await FluentActions.Invoking(() => _controller.GetPaymentFeeDetailsByAccreditationMaterialId(accreditationMaterialId))
                            .Should().ThrowAsync<Exception>()
                            .WithMessage("Service error");
    }

    [TestMethod]
    public async Task MarkAccreditationMaterialStatusAsDulyMade_ShouldReturnNoContent_WhenValidRequest()
    {
        // Arrange
        var requestDto = _fixture.Create<AccreditationMarkAsDulyMadeWithUserIdDto>();

        _mockMarkAsDulyMadeRequestValidator
            .Setup(v => v.ValidateAsync(requestDto, default))
            .ReturnsAsync(new ValidationResult());

        _mockAccreditationService
            .Setup(s => s.MarkAccreditationMaterialStatusAsDulyMade(It.IsAny<Guid>(), requestDto))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.MarkAccreditationMaterialStatusAsDulyMade(requestDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [TestMethod]
    public async Task MarkAccreditationMaterialStatusAsDulyMade_ShouldThrowValidationException_WhenValidationFails()
    {
        // Arrange
        var validator = new InlineValidator<AccreditationMarkAsDulyMadeRequestDto>();
        validator.RuleFor(x => x.DeterminationDate).Must(_ => false).WithMessage("Invalid");

        _controller = new AccreditationsController(
            _mockReprocessorExporterService.Object,
            _mockAccreditationService.Object,
            validator,
            _mockRegulatorApplicationValidator.Object,
            _mockLogger.Object
        );

        var requestDto = new AccreditationMarkAsDulyMadeWithUserIdDto();

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.MarkAccreditationMaterialStatusAsDulyMade(requestDto)
        ).Should().ThrowAsync<ValidationException>();
    }

    [TestMethod]
    public async Task UpdateAccreditationMaterialTaskStatus_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = _fixture.Create<UpdateAccreditationMaterialTaskStatusDto>();
        var validationResult = new ValidationResult();

        _mockRegulatorApplicationValidator
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);

        _mockAccreditationService
            .Setup(s => s.UpdateAccreditationMaterialTaskStatus(It.IsAny<Guid>(), request))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateAccreditationMaterialTaskStatus(request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [TestMethod]
    public async Task UpdateAccreditationMaterialTaskStatus_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var validator = new InlineValidator<UpdateAccreditationMaterialTaskStatusDto>();
        validator.RuleFor(x => x.RegistrationMaterialId).NotEmpty().WithMessage("RegistrationMaterialId is required");
        validator.RuleFor(x => x.TaskId).NotEmpty().WithMessage("TaskName is required");
        validator.RuleFor(x => x.TaskStatus).IsInEnum().WithMessage("Status is required");
        
        _controller = new AccreditationsController(
            _mockReprocessorExporterService.Object,
            _mockAccreditationService.Object,
            _mockMarkAsDulyMadeRequestValidator.Object,
            validator,
            _mockLogger.Object
        );

        var invalidRequest = new UpdateAccreditationMaterialTaskStatusDto
        {
            RegistrationMaterialId = Guid.NewGuid(),
            TaskStatus = 0,
            Comments = "Testing",
            TaskId = 0
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.UpdateAccreditationMaterialTaskStatus(invalidRequest)
        ).Should().ThrowAsync<ValidationException>()
         .WithMessage("*is required*");
    }
}
