using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.Core.Clients;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.TradeAntiVirus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers.FileUpload
{
    [TestClass]
    public class FileUploadControllerTests : Controller
    {
        private Mock<ILogger<AntivirusClient>> _loggerMock;
        readonly HttpClient httpClient = new HttpClient() { BaseAddress = new Uri("") };
        private AntivirusClient virusClient;
        private IAntivirusService _antivirusService;
        private AntivirusApiConfig _antivirusApiConfig;
        IOptions<AntivirusApiConfig> antivirusApiConfigSettings;
        private FileUploadController _sut;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<AntivirusClient>>();
            virusClient = new AntivirusClient(httpClient, _loggerMock.Object);
            _antivirusApiConfig = new AntivirusApiConfig()
            { 
                
            };

            antivirusApiConfigSettings = Options.Create(_antivirusApiConfig);
            _antivirusService = new AntivirusService(virusClient, antivirusApiConfigSettings);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, "testuser@test.com"),
            }, "Test"));

            _sut = new FileUploadController(_antivirusService);
            _sut.ControllerContext = new ControllerContext();
            _sut.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
        }

        [TestMethod]
        public async Task TestFileUpload()
        {
            var fileUploadRequest = new FileUploadRequest()
            {
                SubmissionType = SubmissionType.Registration,
                FileContent = Encoding.ASCII.GetBytes("Test file content"),
                FileName = "TestFile.csv",
                FileId = Guid.NewGuid()
            };

            var data = await _sut.UploadFile(fileUploadRequest);
        }

    }
}
