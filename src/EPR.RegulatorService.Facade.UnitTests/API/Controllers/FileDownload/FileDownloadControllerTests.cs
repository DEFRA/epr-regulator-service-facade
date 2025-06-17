using System.Net;
using System.Security.Claims;
using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Exceptions;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.Models.Submissions.Events;
using EPR.RegulatorService.Facade.Core.Models.TradeAntiVirus;
using EPR.RegulatorService.Facade.Core.Services.BlobStorage;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using EPR.RegulatorService.Facade.Core.TradeAntiVirus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers.FileDownload
{
    [TestClass]
    public class FileDownloadControllerTests : Controller
    {
        private const string SubmissionsServiceError = "There was an error communicating with the submissions API.";
        private const string BlobStorageServiceError = "Error occurred during download from blob storage";
        private const string FileInfectedError = "The file was found but it was flagged as infected. It will not be downloaded.";
        private readonly Mock<IBlobStorageService> _mockBlobStorageService = new();        
        private readonly Mock<IAntivirusService> _mockAntiVirusService = new();
        private readonly Mock<ISubmissionService> _mockSubmissionService = new();
        private readonly Mock<IOptions<BlobStorageConfig>> _mockBlobStorageConfig = new();
        private readonly Mock<IOptions<AntivirusApiConfig>> _mockAntivirusApiConfig = new();
        private readonly string _blobName = new Guid("34f0251a-fc7b-4792-8bb3-684d38eb9e0c").ToString();
        private FileDownloadRequest _fileDownloadRequest;
        private FileDownloadController _sut;
        private HttpResponseMessage _cleanAntiVirusResponse;
        private HttpResponseMessage _maliciousAntiVirusResponse;
        private HttpResponseMessage _failedSubmissionResponse;
        private HttpResponseMessage _passedSubmissionResponse;
        private const string RegistrationContainerName = "test-container";
        private readonly BlobStorageConfig _blobStorageConfig = new()
        {
            PomContainerName = "pom-test-container",
            RegistrationContainerName = RegistrationContainerName
        };
        private readonly AntivirusApiConfig _antivirusApiConfig = new()
        {
            CollectionSuffix = "test-suffix",
            PersistFile = true
        };
        

        [TestInitialize]
        public void Setup()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, "testuser@test.com"),
            }, "Test"));
            _mockBlobStorageConfig.Setup(x => x.Value).Returns(_blobStorageConfig);
            _mockAntivirusApiConfig.Setup(x => x.Value).Returns(_antivirusApiConfig);

            _sut = new FileDownloadController(_mockBlobStorageService.Object,
                _mockSubmissionService.Object,
                _mockAntiVirusService.Object,
                _mockBlobStorageConfig.Object);
            _sut.ControllerContext = new ControllerContext();
            _sut.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
            

            _fileDownloadRequest = new FileDownloadRequest()
            {
                SubmissionType = SubmissionType.Registration,
                BlobName = _blobName,
                FileName = "TestFile.csv",
                FileId = Guid.NewGuid(),
                SubmissionId = Guid.NewGuid()
            };

            _cleanAntiVirusResponse = new HttpResponseMessage()
            {
                Content = new StringContent("Content-Scan: Clean")
            };

            _maliciousAntiVirusResponse = new HttpResponseMessage()
            {
                Content = new StringContent("Content-Scan: Malicious")
            };

            _failedSubmissionResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.Forbidden
            };

            _passedSubmissionResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK
            };
        }

        [TestMethod]
        public async Task Should_return_BlobStorageServiceException_when_BlobStorageServiceFails()
        {
            // Arrange
            _mockBlobStorageService
                .Setup(x => x.DownloadFileStreamAsync(RegistrationContainerName, _blobName))
                .Throws(new BlobStorageServiceException(BlobStorageServiceError));

            // Act and Assert
            await _sut
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
                .Setup(x => x.DownloadFileStreamAsync(RegistrationContainerName, _blobName))
                .ReturnsAsync(new MemoryStream());

            _mockAntiVirusService.Setup(x => x.SendFile(
                It.IsAny<AntiVirusDetails>(),
                It.IsAny<MemoryStream>()))
                .Throws<HttpRequestException>();

            // Act and Assert
            await _sut
                .Invoking(x => x.DownloadFile(_fileDownloadRequest))
                .Should()
                .ThrowAsync<HttpRequestException>();
        }

        [TestMethod]
        public async Task Should_return_BadRequestObjectResult_when_SubmissionsServiceFails()
        {
            // Arrange
            _mockBlobStorageService
                .Setup(x => x.DownloadFileStreamAsync(RegistrationContainerName, _blobName))
                .ReturnsAsync(new MemoryStream());

            _mockAntiVirusService.Setup(x => x.SendFile(                    
                    It.IsAny<AntiVirusDetails>(),
                    It.IsAny<MemoryStream>()))
                .ReturnsAsync(_cleanAntiVirusResponse);

            _mockSubmissionService.Setup(x =>
                x.CreateSubmissionEvent(It.IsAny<Guid>(), It.IsAny<FileDownloadCheckEvent>(), It.IsAny<Guid>()))
                .ReturnsAsync(_failedSubmissionResponse);

            // Act
            var result = await _sut.DownloadFile(_fileDownloadRequest) as BadRequestObjectResult;

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            result.Value.Should().Be(SubmissionsServiceError);
        }

        [TestMethod]
        public async Task Should_return_ForbiddenObjectResult_when_AntiVirusServiceReturnsMalicious()
        {
            // Arrange
            _mockBlobStorageService
                .Setup(x => x.DownloadFileStreamAsync(RegistrationContainerName, _blobName))
                .ReturnsAsync(new MemoryStream());

            _mockAntiVirusService.Setup(x => x.SendFile(
                    It.IsAny<AntiVirusDetails>(),
                    It.IsAny<MemoryStream>()))
                .ReturnsAsync(_maliciousAntiVirusResponse);

            _mockSubmissionService.Setup(x =>
                    x.CreateSubmissionEvent(It.IsAny<Guid>(), It.IsAny<FileDownloadCheckEvent>(), It.IsAny<Guid>()))
                .ReturnsAsync(_passedSubmissionResponse);

            // Act
            var result = await _sut.DownloadFile(_fileDownloadRequest);

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
                .Setup(x => x.DownloadFileStreamAsync(RegistrationContainerName, _blobName))
                .ReturnsAsync(new MemoryStream());

            _mockAntiVirusService.Setup(x => x.SendFile(
                    It.IsAny<AntiVirusDetails>(),
                    It.IsAny<MemoryStream>()))
                .ReturnsAsync(_cleanAntiVirusResponse);

            _mockSubmissionService.Setup(x =>
                    x.CreateSubmissionEvent(It.IsAny<Guid>(), It.IsAny<FileDownloadCheckEvent>(), It.IsAny<Guid>()))
                .ReturnsAsync(_passedSubmissionResponse);

            // Act
            var result = await _sut.DownloadFile(_fileDownloadRequest) as FileContentResult;

            // Assert
            result.Should().BeOfType<FileContentResult>();
            result.Should().NotBeNull();
            result.FileDownloadName.Should().Be(_fileDownloadRequest.FileName);
            result.ContentType.Should().Be("text/csv");
        }
    }
}