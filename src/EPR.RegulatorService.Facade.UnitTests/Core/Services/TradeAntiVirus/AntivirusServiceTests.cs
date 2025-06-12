using EPR.RegulatorService.Facade.Core.Clients;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.TradeAntiVirus;
using EPR.RegulatorService.Facade.Core.TradeAntiVirus;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Services.TradeAntiVirus
{
    [TestClass]
    public class AntivirusServiceTests
    {
        private Mock<IAntivirusClient> _mockAntivirusClient = null!;
        private readonly Mock<IOptions<AntivirusApiConfig>> _mockAntivirusApiConfig = new();
        private AntivirusService _antivirusService = null!;

        private readonly AntivirusApiConfig _antivirusApiConfig = new()
        {
            CollectionSuffix = "_Test",
            PersistFile = true
        };

        IOptions<AntivirusApiConfig> antivirusApiConfigSettings;

        [TestInitialize]
        public void Setup()
        {
            _mockAntivirusClient = new Mock<IAntivirusClient>();
            _mockAntivirusApiConfig.Setup(x => x.Value).Returns(_antivirusApiConfig);
            antivirusApiConfigSettings = Options.Create(_antivirusApiConfig);

            _antivirusService = new AntivirusService(_mockAntivirusClient.Object ,antivirusApiConfigSettings);
        }

        [TestMethod]
        public async Task SendFile_ShouldCallVirusScanFileWithCorrectParameters()
        {
            // Arrange
            var storageContainerName = "pom_Test";            
            var fileId = Guid.NewGuid();
            var fileName = "testfile.txt";
            var fileStream = new MemoryStream();
            var userId = Guid.NewGuid();
            var email = "user@example.com";

            var expectedFileDetails = new FileDetails
            {
                Key = fileId,
                Extension = ".txt",
                FileName = "testfile",
                Collection = storageContainerName,
                UserId = userId,
                UserEmail = email,
                PersistFile = true,
                Content = null!
            };

            var antiVirusFileDetails = new AntiVirusDetails
            {
                FileId = fileId,
                FileName = fileName,
                SubmissionType = SubmissionType.Producer,
                UserId = userId,
                UserEmail = email
            };  

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            _mockAntivirusClient.Setup(c => c.VirusScanFile(It.Is<FileDetails>(f =>
                f.Key == expectedFileDetails.Key &&
                f.Extension == expectedFileDetails.Extension &&
                f.FileName == expectedFileDetails.FileName &&
                f.Collection == expectedFileDetails.Collection &&
                f.UserId == expectedFileDetails.UserId &&
                f.UserEmail == expectedFileDetails.UserEmail &&
                f.PersistFile == expectedFileDetails.PersistFile &&
                f.Content == null!
            ), fileName, fileStream)).ReturnsAsync(response);

            // Act
            var result = await _antivirusService.SendFile(antiVirusFileDetails, fileStream);

            // Assert
            result.Should().Be(response);
            _mockAntivirusClient.Verify(c => c.VirusScanFile(It.IsAny<FileDetails>(), fileName, fileStream), Times.Once);
        }

        [TestMethod]
        public async Task SendFile_ShouldThrowException_WhenVirusScanFileFails()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var fileName = "testfile.txt";
            var fileStream = new MemoryStream();
            var userId = Guid.NewGuid();
            var email = "user@example.com";

            var antiVirusFileDetails = new AntiVirusDetails
            {
                FileId = fileId,
                FileName = fileName,
                SubmissionType = SubmissionType.Producer,
                UserId = userId,
                UserEmail = email
            };

            _mockAntivirusClient.Setup(c => c.VirusScanFile(It.IsAny<FileDetails>(), fileName, fileStream))
                .ThrowsAsync(new HttpRequestException("Virus scan failed"));

            // Act
            Func<Task> act = async () => await _antivirusService.SendFile(antiVirusFileDetails, fileStream);

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>().WithMessage("Virus scan failed");
        }
    }
}
