using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Extensions;
using FluentAssertions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EPR.RegulatorService.Facade.UnitTests.Enums
{
    [TestClass]
    public class EnumsTests
    {
        [TestMethod]
        public async Task CheckTheEnumeratedTypeReturn()
        {
            // Arrange
            SubmissionType submissionType = new();
            submissionType = SubmissionType.Registration;

            // Act
            var result = EnumExtensions.GetDisplayName(submissionType);
                        
            // Assert
            result.ToLower().Should().Be(SubmissionType.Registration.ToString().ToLower());
        }

        [TestMethod]
        public async Task CheckTheEnumeratedTypeReturnEmptyString()
        {
            // Arrange
            SubmissionType submissionType = new();

            // Act
            var result = EnumExtensions.GetDisplayName(submissionType);

            // Assert
            result.Should().Be(string.Empty);
        }
    }
}