using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.Core.Enums;
using FluentAssertions;

namespace EPR.RegulatorService.Facade.UnitTests.API;

[TestClass]
public class RegistrationStatusExtentionsTests
{
    [TestMethod]
    [DataRow(RegistrationStatus.None, RegulatorDecision.None)]
    [DataRow(RegistrationStatus.Pending, RegulatorDecision.None)]
    public void Should_Return_Correct_RegulatorDesision(RegistrationStatus registrationStatus, RegulatorDecision regulatorDecision)
    {
        // Arrange

        // Act
        var result = registrationStatus.GetRegulatorDecision();

        // Assert
        result.Should().Be(regulatorDecision);
    }
}