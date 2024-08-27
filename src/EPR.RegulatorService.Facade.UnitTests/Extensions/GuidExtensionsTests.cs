using EPR.RegulatorService.Facade.Core.Extensions;
using FluentAssertions;

namespace EPR.RegulatorService.Facade.UnitTests.Extensions
{
    [TestClass]
    public class GuidExtensionsTests
    {
        [TestMethod]
        public void CheckValidGuid_ValidGuid_ReturnsTrue()
        {
            // Arrange
            var userGuid = Guid.NewGuid();

            // Act
            var result = userGuid.IsValidGuid();
                        
            // Assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void CheckValidGuid_EmptyGuid_ReturnsFalse()
        {
            var userGuid = Guid.Empty;

            // Act
            var result = userGuid.IsValidGuid();

            // Assert
            result.Should().Be(false);
        }
    }
}