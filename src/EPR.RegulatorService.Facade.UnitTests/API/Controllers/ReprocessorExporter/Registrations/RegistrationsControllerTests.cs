using AutoFixture;
using EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Exceptions;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.TradeAntiVirus;
using EPR.RegulatorService.Facade.Core.Services.BlobStorage;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.TradeAntiVirus;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers.ReprocessorExporter.Registrations;

[TestClass]
public class RegistrationsControllerTests
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
        ReprocessorExporterRegistrationContainerName = RegistrationContainerName,
    };    

    IOptions<BlobStorageConfig> blobStorageConfigSettings;

    private Mock<IReprocessorExporterService> _mockReprocessorExporterService = null!;
    private Mock<IValidator<UpdateRegulatorRegistrationTaskDto>> _mockRegulatorRegistrationValidator = null!;
    private Mock<IValidator<UpdateRegulatorApplicationTaskDto>> _mockRegulatorApplicationValidator = null!; 
    private Mock<IValidator<QueryNoteRequestDto>> _mockQueryNoteRequestDtoValidator = null!;
    private Mock<ILogger<RegistrationsController>> _mockLogger = null!;
    private Fixture _fixture = null!;
    private RegistrationsController _controller;

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
        _mockRegulatorRegistrationValidator = new Mock<IValidator<UpdateRegulatorRegistrationTaskDto>>();
        _mockRegulatorApplicationValidator = new Mock<IValidator<UpdateRegulatorApplicationTaskDto>>();
        _mockQueryNoteRequestDtoValidator = new Mock<IValidator<QueryNoteRequestDto>>();
        _mockLogger = new Mock<ILogger<RegistrationsController>>();
        _fixture = new Fixture();

        _controller = new RegistrationsController(
            _mockReprocessorExporterService.Object,
            _mockRegulatorRegistrationValidator.Object,
            _mockRegulatorApplicationValidator.Object,
            _mockQueryNoteRequestDtoValidator.Object,
            _mockBlobStorageService.Object, 
            _mockAntiVirusService.Object, 
            blobStorageConfigSettings, 
            _mockLogger.Object
        );

        _controller.ControllerContext = new ControllerContext();
        _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
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

        _mockReprocessorExporterService
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
            _mockReprocessorExporterService.Object,
            validator,
            _mockRegulatorApplicationValidator.Object,
            _mockQueryNoteRequestDtoValidator.Object,
            _mockBlobStorageService.Object,
            _mockAntiVirusService.Object,
            blobStorageConfigSettings,
            _mockLogger.Object
        );

        var invalidRequest = new UpdateRegulatorRegistrationTaskDto
        {
            RegistrationId = Guid.NewGuid(),
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

        _mockReprocessorExporterService
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
            _mockReprocessorExporterService.Object,
            _mockRegulatorRegistrationValidator.Object,
            validator,
            _mockQueryNoteRequestDtoValidator.Object,
            _mockBlobStorageService.Object,
            _mockAntiVirusService.Object,
            blobStorageConfigSettings,
            _mockLogger.Object
        );

        var invalidRequest = new UpdateRegulatorApplicationTaskDto
        {
            RegistrationMaterialId = Guid.NewGuid(),
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
        var registrationId = Guid.NewGuid();
        _mockReprocessorExporterService.Setup(service => service.GetRegistrationByRegistrationId(registrationId))
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
    public async Task GetSiteAddressByRegistrationId_ShouldReturnOk_WithExpectedResult()
    {
        // Arrange
        var registrationId = Guid.NewGuid();
        var expectedDto = _fixture.Create<SiteAddressDetailsDto>();

        _mockReprocessorExporterService
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
        var registrationId = Guid.NewGuid();
        _mockReprocessorExporterService
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
        var registrationId = Guid.NewGuid();
        var expectedDto = _fixture.Create<MaterialsAuthorisedOnSiteDto>();

        _mockReprocessorExporterService
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
        var registrationId = Guid.NewGuid();
        _mockReprocessorExporterService
            .Setup(service => service.GetAuthorisedMaterialByRegistrationId(registrationId))
            .ThrowsAsync(new Exception("Service error"));

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.GetAuthorisedMaterialByRegistrationId(registrationId)
        ).Should().ThrowAsync<Exception>()
         .WithMessage("Service error");
    }

    [TestMethod]
    public async Task SaveApplicationTaskQueryNotes_ShouldReturnNoContent_WhenValidRequest()
    {
        // Arrange
        var regulatorApplicationTaskStatusId = Guid.NewGuid();
        var requestDto = _fixture.Create<QueryNoteRequestDto>();

        _mockQueryNoteRequestDtoValidator
            .Setup(v => v.ValidateAsync(requestDto, default))
            .ReturnsAsync(new ValidationResult());

        _mockReprocessorExporterService
            .Setup(s => s.SaveApplicationTaskQueryNotes(regulatorApplicationTaskStatusId, It.IsAny<Guid>(), requestDto))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.SaveApplicationTaskQueryNotes(regulatorApplicationTaskStatusId, requestDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [TestMethod]
    public async Task SaveApplicationTaskQueryNotes_ShouldThrowValidationException_WhenValidationFails()
    {
        // Arrange
        var validator = new InlineValidator<QueryNoteRequestDto>();
        validator.RuleFor(x => x.Note).NotEmpty().WithMessage("The Query Note field is required.");

        _controller = new RegistrationsController(
            _mockReprocessorExporterService.Object,
            _mockRegulatorRegistrationValidator.Object,
            _mockRegulatorApplicationValidator.Object,
            validator,
            _mockBlobStorageService.Object,
            _mockAntiVirusService.Object,
            blobStorageConfigSettings,
            _mockLogger.Object
        );

        var requestDto = new QueryNoteRequestDto();

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.SaveApplicationTaskQueryNotes(Guid.NewGuid(), requestDto)
        ).Should().ThrowAsync<ValidationException>();
    }

    [TestMethod]
    public async Task SaveRegistrationTaskQueryNotes_ShouldReturnNoContent_WhenValidRequest()
    {
        // Arrange
        var regulatorRegistrationTaskStatusId = Guid.NewGuid();
        var requestDto = _fixture.Create<QueryNoteRequestDto>();

        _mockQueryNoteRequestDtoValidator
            .Setup(v => v.ValidateAsync(requestDto, default))
            .ReturnsAsync(new ValidationResult());

        _mockReprocessorExporterService
            .Setup(s => s.SaveRegistrationTaskQueryNotes(regulatorRegistrationTaskStatusId, It.IsAny<Guid>(), requestDto))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.SaveRegistrationTaskQueryNotes(regulatorRegistrationTaskStatusId, requestDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [TestMethod]
    public async Task SaveRegistrationTaskQueryNotes_ShouldThrowValidationException_WhenValidationFails()
    {
        // Arrange
        var validator = new InlineValidator<QueryNoteRequestDto>();
        validator.RuleFor(x => x.Note).NotEmpty().WithMessage("The Query Note field is required.");

        _controller = new RegistrationsController(
            _mockReprocessorExporterService.Object,
            _mockRegulatorRegistrationValidator.Object,
            _mockRegulatorApplicationValidator.Object,
            validator,
            _mockBlobStorageService.Object,
            _mockAntiVirusService.Object,
            blobStorageConfigSettings,
            _mockLogger.Object
        );

        var requestDto = new QueryNoteRequestDto();

        // Act & Assert
        await FluentActions.Invoking(() =>
            _controller.SaveRegistrationTaskQueryNotes(Guid.NewGuid(), requestDto)
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
}
