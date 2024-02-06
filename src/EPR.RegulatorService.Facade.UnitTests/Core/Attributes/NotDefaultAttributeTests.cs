using EPR.RegulatorService.Facade.Core.Attributes;
using FluentAssertions;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Attributes
{
    [TestClass]
    public class NotDefaultAttributeTests
    {

        [TestMethod]
        public void IsValid_ReturnsTrue_WithNullValue()
        {
            //Arrange
            var sut = new NotDefaultAttribute();

            //Act
            var result = sut.IsValid(null);

            //Assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void IsValid_ReturnsTrue_WithTypeOtherThanNumber()
        {
            //Arrange
            var sut = new NotDefaultAttribute();

            //Act
            var result = sut.IsValid("Test");

            //Assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void IsValid_ReturnsTrue_WithTypeOfNumberNotZero()
        {
            //Arrange
            var sut = new NotDefaultAttribute();

            //Act
            var result = sut.IsValid(2);

            //Assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void IsValid_ReturnsFalse_WithTypeOfNumberNotZero()
        {
            //Arrange
            var sut = new NotDefaultAttribute();

            //Act
            var result = sut.IsValid(0);

            //Assert
            result.Should().Be(false);
        }
    }
}