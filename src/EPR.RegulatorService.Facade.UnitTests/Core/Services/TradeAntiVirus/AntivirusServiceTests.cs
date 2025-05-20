using EPR.RegulatorService.Facade.Core.Clients;
using EPR.RegulatorService.Facade.Core.Models.TradeAntiVirus;
using EPR.RegulatorService.Facade.Core.TradeAntiVirus;
using FluentAssertions;
using Moq;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Services.TradeAntiVirus
{
    [TestClass]
    public class AntivirusServiceTests
    {
        private Mock<IAntivirusClient> _mockAntivirusClient = null!;
        private AntivirusService _antivirusService = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockAntivirusClient = new Mock<IAntivirusClient>();
            _antivirusService = new AntivirusService(_mockAntivirusClient.Object);
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
            var result = await _antivirusService.SendFile(expectedFileDetails, fileName, fileStream);

            // Assert
            result.Should().Be(response);
            _mockAntivirusClient.Verify(c => c.VirusScanFile(It.IsAny<FileDetails>(), fileName, fileStream), Times.Once);
        }

        [TestMethod]
        public async Task SendFile_ShouldThrowException_WhenVirusScanFileFails()
        {
            // Arrange
            var storageContainerName = "Producer"; // Replace with a valid enum value
            var fileId = Guid.NewGuid();
            var fileName = "testfile.txt";
            var fileStream = new MemoryStream();
            var userId = Guid.NewGuid();
            var email = "user@example.com";

            var fileDetails = new FileDetails
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

            _mockAntivirusClient.Setup(c => c.VirusScanFile(It.IsAny<FileDetails>(), fileName, fileStream))
                .ThrowsAsync(new HttpRequestException("Virus scan failed"));

            // Act
            Func<Task> act = async () => await _antivirusService.SendFile(fileDetails, fileName, fileStream);

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>().WithMessage("Virus scan failed");
        }
    }
}
