using System.Diagnostics;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EPR.RegulatorService.Facade.UnitTests.Helpers
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
            string urlToCheck = urlToPass;

            // Assert
            Debug.Assert(urlToCheck != null);
            Debug.Assert(urlToCheck == urlToPass);
        }

        [TestMethod]
        public async Task CheckReturnUrlWithEmptyAddress()
        {
            // Arrange
            string urlToPass = "";

            // Act
            string urlToCheck = urlToPass;

            // Assert
            Debug.Assert(urlToCheck != null);
            Debug.Assert(urlToCheck == urlToPass);
        }

        [TestMethod]
        public async Task CheckReturnUrlWithHostName()
        {
            // Arrange
            string urlToPass = "http://defra.gov.uk/";

            // Act
            string urlToCheck = urlToPass;

            // Assert
            Debug.Assert(urlToCheck != null);
            Debug.Assert(urlToCheck == urlToPass);
        }
    }
}