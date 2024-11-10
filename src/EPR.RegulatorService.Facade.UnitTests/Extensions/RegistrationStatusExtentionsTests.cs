using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Extensions;
using FluentAssertions;

namespace EPR.RegulatorService.Facade.UnitTests.Extensions;

[TestClass]
public class RegistrationStatusExtentionTests
{
    [TestMethod]
    [DataRow(RegistrationSubmissionStatus.None, RegulatorDecision.None)]
    [DataRow(RegistrationSubmissionStatus.Pending, RegulatorDecision.None)]
    public void Should_Return_Correct_RegulatorDecision(RegistrationSubmissionStatus registrationStatus, RegulatorDecision regulatorDecision)
    {
        // Arrange

        // Act
        var result = registrationStatus.GetRegulatorDecision();

        // Assert
        result.Should().Be(regulatorDecision);
    }
}