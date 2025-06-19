using AutoFixture;
using EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Exceptions;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.TradeAntiVirus;
using EPR.RegulatorService.Facade.Core.Services.BlobStorage;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.TradeAntiVirus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using System.Security.Claims;
using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers.ReprocessorExporter.Registrations;

[TestClass]
public class AccreditationsControllerTests
{
    private const string BlobStorageServiceError = "Error occurred during download from blob storage";
    private const string FileInfectedError = "The file was found but it was flagged as infected. It will not be downloaded.";
    private readonly Mock<IBlobStorageService> _mockBlobStorageService = new();
    private readonly Mock<IAntivirusService> _mockAntiVirusService = new();
    private readonly Mock<IOptions<BlobStorageConfig>> _mockBlobStorageConfig = new();
    private FileDownloadRequestDto _fileDownloadRequest = new();
    private HttpResponseMessage _cleanAntiVirusResponse = new();
    private HttpResponseMessage _maliciousAntiVirusResponse = new();
    private const string RegistrationContainerName = "test-container";

    private readonly BlobStorageConfig _blobStorageConfig = new()
    {
        PomContainerName = "pom-test-container",
        RegistrationContainerName = RegistrationContainerName,
        ReprocessorExporterAccreditationContainerName = RegistrationContainerName,
    };

    IOptions<BlobStorageConfig> blobStorageConfigSettings;

    private Mock<IReprocessorExporterService> _mockReprocessorExporterService = null!;
    private Mock<ILogger<AccreditationsController>> _mockLogger = null!;
    private Fixture _fixture = null!;
    private AccreditationsController _controller = null!;
    private Mock<IValidator<MarkAsDulyMadeRequestDto>> _mockMarkAsDulyMadeRequestValidator = null!;
    private Mock<IValidator<UpdateAccreditationTaskStatusDto>> _mockRegulatorApplicationValidator = null!;
    private Mock<IValidator<OfflinePaymentRequestDto>> _mockOfflinePaymentRequestDtoValidator = null!;
    private Mock<IValidator<OfflinePaymentRequestDto>> _mockOfflinePaymentRequestValidator = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "testuser@test.com"),
        }, "Test"));

        _mockBlobStorageConfig.Setup(x => x.Value).Returns(_blobStorageConfig);
        blobStorageConfigSettings = Options.Create(_blobStorageConfig);

        _fileDownloadRequest = new FileDownloadRequestDto()
        {
            FileName = "TestFile.csv",
            FileId = Guid.NewGuid()
        };

        _cleanAntiVirusResponse = new HttpResponseMessage()
        {
            Content = new StringContent("Content-Scan: Clean")
        };

        _maliciousAntiVirusResponse = new HttpResponseMessage()
        {
            Content = new StringContent("Content-Scan: Malicious")
        };

        _mockReprocessorExporterService = new Mock<IReprocessorExporterService>();
        _mockLogger = new Mock<ILogger<AccreditationsController>>();
        _mockMarkAsDulyMadeRequestValidator = new Mock<IValidator<MarkAsDulyMadeRequestDto>>();
        _mockRegulatorApplicationValidator = new Mock<IValidator<UpdateAccreditationTaskStatusDto>>();
        _mockOfflinePaymentRequestDtoValidator = new Mock<IValidator<OfflinePaymentRequestDto>>();
        _mockOfflinePaymentRequestValidator = new Mock<IValidator<OfflinePaymentRequestDto>>();

        _fixture = new Fixture();
        _controller = new AccreditationsController(_mockReprocessorExporterService.Object,
                                                _mockMarkAsDulyMadeRequestValidator.Object,
                                                _mockRegulatorApplicationValidator.Object,
                                                _mockOfflinePaymentRequestDtoValidator.Object,
                                                _mockBlobStorageService.Object,
                                                _mockAntiVirusService.Object,
                                                blobStorageConfigSettings,
                                                _mockLogger.Object);

        _controller.ControllerContext = new ControllerContext();
        _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
    }

    [TestMethod]
    public async Task GetRegistrationByIdWithAccreditations_ShouldReturnExpectedResult()
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
    public async Task GetSamplingPlanByAccreditationId_ShouldReturnExpectedResult()
    {
        // Arrange
        var expectedDto = _fixture.Create<AccreditationSamplingPlanDto>();
        var id = Guid.NewGuid();
        _mockReprocessorExporterService.Setup(service => service.GetSamplingPlanByAccreditationId(id))
                                    .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.GetSamplingPlansAsync(id);

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
        Guid id = Guid.NewGuid();
        var expectedDto = _fixture.Create<AccreditationPaymentFeeDetailsDto>();

        _mockReprocessorExporterService.Setup(service => service.GetAccreditationPaymentFeeDetailsByAccreditationId(id))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.GetAccreditationPaymentFeeDetailsByAccreditationId(id);

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
        _mockReprocessorExporterService.Setup(service => service.GetAccreditationPaymentFeeDetailsByAccreditationId(accreditationMaterialId))
            .ThrowsAsync(new Exception("Service error"));

        // Act & Assert
        await FluentActions.Invoking(() => _controller.GetAccreditationPaymentFeeDetailsByAccreditationId(accreditationMaterialId))
            .Should().ThrowAsync<Exception>()
            .WithMessage("Service error");
    }

    [TestMethod]
    public async Task MarkAccreditationMaterialStatusAsDulyMade_ShouldReturnNoContent_WhenValidRequest()
    {
        // Arrange
        var accreditationId = Guid.NewGuid();

        // Arrange
        var requestDto = _fixture.Create<MarkAsDulyMadeRequestDto>();

        _mockMarkAsDulyMadeRequestValidator
            .Setup(v => v.ValidateAsync(requestDto, default))
            .ReturnsAsync(new ValidationResult());

        _mockReprocessorExporterService
            .Setup(s => s.MarkAsDulyMadeByAccreditationId(It.IsAny<Guid>(), It.IsAny<Guid>(), requestDto))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.MarkAsDulyMadeByAccreditationId(accreditationId, requestDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [TestMethod]
    public async Task MarkAccreditationMaterialStatusAsDulyMade_ShouldThrowValidationException_WhenValidationFails()
    {
        // Arrange
        var accreditationId = Guid.NewGuid();
        var validator = new InlineValidator<MarkAsDulyMadeRequestDto>();
        validator.RuleFor(x => x.DeterminationDate).Must(_ => false).WithMessage("Invalid");

        _controller = new AccreditationsController(
            _mockReprocessorExporterService.Object,
            validator,
            _mockRegulatorApplicationValidator.Object,
            _mockOfflinePaymentRequestDtoValidator.Object,
            _mockBlobStorageService.Object,
            _mockAntiVirusService.Object,
            blobStorageConfigSettings,
            _mockLogger.Object
        );

        var requestDto = new MarkAsDulyMadeRequestDto();

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.MarkAsDulyMadeByAccreditationId(accreditationId, requestDto)
        ).Should().ThrowAsync<ValidationException>();
    }

    [TestMethod]
    public async Task UpdateAccreditationMaterialTaskStatus_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = _fixture.Create<UpdateAccreditationTaskStatusDto>();
        var validationResult = new ValidationResult();

        _mockRegulatorApplicationValidator
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);

        _mockReprocessorExporterService
            .Setup(s => s.UpdateRegulatorAccreditationTaskStatus(It.IsAny<Guid>(), request))
            .ReturnsAsync(true);
        
        // Act
        var result = await _controller.UpdateRegulatorAccreditationTaskStatus(request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [TestMethod]
    public async Task UpdateAccreditationMaterialTaskStatus_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var validator = new InlineValidator<UpdateAccreditationTaskStatusDto>();
        validator.RuleFor(x => x.TaskName).NotEmpty().WithMessage("TaskName is required");
        validator.RuleFor(x => x.Status).IsInEnum().WithMessage("Status is required");

        _controller = new AccreditationsController(
            _mockReprocessorExporterService.Object,
            _mockMarkAsDulyMadeRequestValidator.Object,
            validator,
            _mockOfflinePaymentRequestDtoValidator.Object,
            _mockBlobStorageService.Object,
            _mockAntiVirusService.Object,
            blobStorageConfigSettings,
            _mockLogger.Object
        );

        var invalidRequest = new UpdateAccreditationTaskStatusDto
        {
            Status = RegistrationTaskStatus.Queried,
            Comments = "Testing",
            TaskName = ""
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.UpdateRegulatorAccreditationTaskStatus(invalidRequest)
        ).Should().ThrowAsync<ValidationException>()
         .WithMessage("*is required*");
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
        var result = await _controller.SaveAccreditationOfflinePayment(requestDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [TestMethod]
    public async Task SaveOfflinePayment_ShouldThrowValidationException_WhenValidationFails()
    {
        // Arrange
        var validator = new InlineValidator<OfflinePaymentRequestDto>();
        validator.RuleFor(x => x.Amount).Must(_ => false).WithMessage("Invalid");

        _controller = new AccreditationsController(
            _mockReprocessorExporterService.Object,
            _mockMarkAsDulyMadeRequestValidator.Object,
            _mockRegulatorApplicationValidator.Object,
            validator,
            _mockBlobStorageService.Object,
            _mockAntiVirusService.Object,
            blobStorageConfigSettings,
            _mockLogger.Object
        );

        var requestDto = new OfflinePaymentRequestDto();

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.SaveAccreditationOfflinePayment(requestDto)
        ).Should().ThrowAsync<ValidationException>();
    }

    [TestMethod]
    public async Task Should_return_BlobStorageServiceException_when_BlobStorageServiceFails()
    {
        // Arrange
        _mockBlobStorageService
            .Setup(x => x.DownloadFileStreamAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new BlobStorageServiceException(BlobStorageServiceError));

        // Act and Assert
        await _controller
            .Invoking(x => x.DownloadFile(_fileDownloadRequest))
            .Should()
            .ThrowAsync<BlobStorageServiceException>()
            .WithMessage(BlobStorageServiceError);
    }

    [TestMethod]
    public async Task Should_return_HttpRequestException_when_AntiVirusServiceFails()
    {
        // Arrange
        _mockBlobStorageService
            .Setup(x => x.DownloadFileStreamAsync(RegistrationContainerName, It.IsAny<string>()))
            .ReturnsAsync(new MemoryStream());

        _mockAntiVirusService.Setup(x => x.SendFile(
            It.IsAny<AntiVirusDetails>(),
            It.IsAny<MemoryStream>()))
            .Throws<HttpRequestException>();

        // Act and Assert
        await _controller
            .Invoking(x => x.DownloadFile(_fileDownloadRequest))
            .Should()
            .ThrowAsync<HttpRequestException>();
    }

    [TestMethod]
    public async Task Should_return_ForbiddenObjectResult_when_AntiVirusServiceReturnsMalicious()
    {
        // Arrange
        _mockBlobStorageService
            .Setup(x => x.DownloadFileStreamAsync(RegistrationContainerName, It.IsAny<string>()))
            .ReturnsAsync(new MemoryStream());

        _mockAntiVirusService.Setup(x => x.SendFile(
            It.IsAny<AntiVirusDetails>(),            
            It.IsAny<MemoryStream>()))
            .ReturnsAsync(_maliciousAntiVirusResponse);

        // Act
        var result = await _controller.DownloadFile(_fileDownloadRequest);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        ((ObjectResult)result).StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        ((ObjectResult)result).Value.Should().Be(FileInfectedError);
    }

    [TestMethod]
    public async Task Should_return_FileContentResult_when_AntiVirusServiceReturnsClean()
    {
        // Arrange
        _mockBlobStorageService
            .Setup(x => x.DownloadFileStreamAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new MemoryStream());

        _mockAntiVirusService.Setup(x => x.SendFile(
                It.IsAny<AntiVirusDetails>(),                
                It.IsAny<MemoryStream>()))
            .ReturnsAsync(_cleanAntiVirusResponse);

        // Act
        var result = await _controller.DownloadFile(_fileDownloadRequest) as FileContentResult;

        // Assert
        result.Should().BeOfType<FileContentResult>();
        result.Should().NotBeNull();
        result.FileDownloadName.Should().Be(_fileDownloadRequest.FileName);
        result.ContentType.Should().Be("text/csv");
    }

    [TestMethod]
    public async Task GetBusinessPlanAsync_ReturnsOkResult_WithBusinessPlan()
    {
        // Arrange
        var accreditationId = Guid.NewGuid();
        var expectedBusinessPlan = new AccreditationBusinessPlanDto();

        _mockReprocessorExporterService
            .Setup(s => s.GetBusinessPlanByAccreditationId(accreditationId))
            .ReturnsAsync(expectedBusinessPlan);

        // Act
        var result = await _controller.GetBusinessPlanAsync(accreditationId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(expectedBusinessPlan, okResult.Value);
    }

    [TestMethod]
    public async Task GetBusinessPlanAsync_ThrowsException_WhenServiceFails()
    {
        // Arrange
        var accreditationId = Guid.NewGuid();
        var expectedMessage = "Service failure";

        _mockReprocessorExporterService
            .Setup(s => s.GetBusinessPlanByAccreditationId(accreditationId))
            .ThrowsAsync(new Exception(expectedMessage));

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<Exception>(async () =>
        {
            await _controller.GetBusinessPlanAsync(accreditationId);
        });

        Assert.AreEqual(expectedMessage, exception.Message);
    }
}
