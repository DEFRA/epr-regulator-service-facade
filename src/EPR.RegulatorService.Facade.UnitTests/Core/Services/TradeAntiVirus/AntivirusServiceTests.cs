using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private AntivirusService _antivirusService = null!;
        private AntivirusApiConfig _antivirusApiConfig = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockAntivirusClient = new Mock<IAntivirusClient>();
            _antivirusApiConfig = new AntivirusApiConfig
            {
                PersistFile = true,
                CollectionSuffix = "_Test"
            };
            var options = Options.Create(_antivirusApiConfig);
            _antivirusService = new AntivirusService(_mockAntivirusClient.Object, options);
        }

        [TestMethod]
        public async Task SendFile_ShouldCallVirusScanFileWithCorrectParameters()
        {
            // Arrange
            var submissionType = SubmissionType.Producer;
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
                Collection = "pom_Test",
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
            var result = await _antivirusService.SendFile(submissionType, fileId, fileName, fileStream, userId, email);

            // Assert
            result.Should().Be(response);
            _mockAntivirusClient.Verify(c => c.VirusScanFile(It.IsAny<FileDetails>(), fileName, fileStream), Times.Once);
        }

        [TestMethod]
        public async Task SendFile_ShouldThrowException_WhenVirusScanFileFails()
        {
            // Arrange
            var submissionType = SubmissionType.Producer; // Replace with a valid enum value
            var fileId = Guid.NewGuid();
            var fileName = "testfile.txt";
            var fileStream = new MemoryStream();
            var userId = Guid.NewGuid();
            var email = "user@example.com";

            _mockAntivirusClient.Setup(c => c.VirusScanFile(It.IsAny<FileDetails>(), fileName, fileStream))
                .ThrowsAsync(new HttpRequestException("Virus scan failed"));

            // Act
            Func<Task> act = async () => await _antivirusService.SendFile(submissionType, fileId, fileName, fileStream, userId, email);

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>().WithMessage("Virus scan failed");
        }
    }
}
