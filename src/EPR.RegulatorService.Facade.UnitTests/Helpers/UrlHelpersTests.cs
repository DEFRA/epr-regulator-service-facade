using EPR.RegulatorService.Facade.Core.Helpers;
using System.Diagnostics;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Services.Regulator
{
    [TestClass]
    public class UrlHelpersTests
    {
        private const string BaseAddress = "http://localhost";

        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        public async Task CheckReturnUrl()
        {
            // Arrange
            string urlToPass = BaseAddress;

            // Act
            string urlToCheck = UrlHelpers.CheckRequestURL(urlToPass);

            // Assert
            Debug.Assert(urlToCheck != null);
            Debug.Assert(urlToCheck == urlToPass);
        }
    }
}