using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.Core.Enums;
using FluentAssertions;

namespace EPR.RegulatorService.Facade.UnitTests.API;

[TestClass]
public class RegistrationStatusExtentionsTests
{
    [TestMethod]
    [DataRow(RegistrationSubmissionStatus.None, RegulatorDecision.None)]
    [DataRow(RegistrationSubmissionStatus.Pending, RegulatorDecision.None)]
    public void Should_Return_Correct_RegulatorDesision(RegistrationSubmissionStatus registrationStatus, RegulatorDecision regulatorDecision)
    {
        // Arrange

        // Act
        var result = registrationStatus.GetRegulatorDecision();

        // Assert
        result.Should().Be(regulatorDecision);
    }
}