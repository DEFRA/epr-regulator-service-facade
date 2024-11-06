using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Extensions;
using FluentAssertions;

namespace EPR.RegulatorService.Facade.UnitTests.Enums
{
    [TestClass]
    public class GuidExtenstionsTests
    {
        [TestMethod]
        public async Task CheckTheEnumeratedTypeReturn()
        {
            // Arrange
            var submissionType = SubmissionType.Registration;

            // Act
            var result = EnumExtensions.GetDisplayName<SubmissionType>(submissionType);
                        
            // Assert
            result.ToLower().Should().Be(SubmissionType.Registration.ToString().ToLower());
        }

        [TestMethod]
        public async Task CheckTheEnumeratedTypeReturnEmptyString()
        {
            // Arrange
            SubmissionType submissionType = new();

            // Act
            var result = EnumExtensions.GetDisplayName<SubmissionType>(submissionType);

            // Assert
            result.Should().Be(string.Empty);
        }
    }
}