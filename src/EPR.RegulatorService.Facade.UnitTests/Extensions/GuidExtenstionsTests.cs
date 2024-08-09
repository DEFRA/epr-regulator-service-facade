using EPR.RegulatorService.Facade.Core.Extensions;
using FluentAssertions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EPR.RegulatorService.Facade.UnitTests.Extensions
{
    [TestClass]
    public class GuidExtenstionsTests
    {
        [TestMethod]
        public async Task CheckValidGuid_ValidGuid_ReturnsTrue()
        {
            // Arrange
            Guid userGuid = Guid.NewGuid();

            // Act
            var result = userGuid.IsValidGuid();
                        
            // Assert
            result.Should().Be(true);
        }

        [TestMethod]
        public async Task CheckValidGuid_EmptyGuid_ReturnsFalse()
        {
            Guid userGuid = Guid.NewGuid();
            userGuid = default(Guid);

            // Act
            var result = userGuid.IsValidGuid();

            // Assert
            result.Should().Be(false);
        }
    }
}