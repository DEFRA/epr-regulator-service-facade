using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentAssertions;

namespace EPR.RegulatorService.Facade.UnitTests.API.Validations.ReprocessorExporter.Registrations;

[TestClass]
public class UpdateAccreditationMaterialTaskStatusDtoValidatorTests
{
    private readonly UpdateAccreditationMaterialTaskStatusDtoValidator _validator = new();

    [TestMethod]
    public void Validator_ShouldPass_WhenAllFieldsAreValid()
    {
        // Arrange
        var request = new UpdateAccreditationTaskStatusDto()
        {
            AccreditationId = Guid.NewGuid(),
            TaskName = "",
            Status = RegistrationTaskStatus.Queried,
            Comments = "All good"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public void Validator_ShouldFail_WhenStatusIsInvalid()
    {
        // Arrange
        var request = new UpdateAccreditationTaskStatusDto
        {
            AccreditationId = Guid.NewGuid(),
            TaskName = "CheckAccreditationStatus",
            Status = (RegistrationTaskStatus)999,
            Comments = ""
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == ValidationMessages.InvalidAccreditationStatus);
    }

    [TestMethod]
    public void Validator_ShouldFail_WhenCommentsRequiredForQueriedStatus_ButMissing()
    {
        // Arrange
        var request = new UpdateAccreditationTaskStatusDto
        {
            AccreditationId = Guid.NewGuid(),
            TaskName = "CheckAccreditationStatus",
            Status = RegistrationTaskStatus.Queried,
            Comments = "", // Required when status is Queried
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == ValidationMessages.RegistrationCommentsRequired);
    }

    [TestMethod]
    public void Validator_ShouldFail_WhenCommentsExceedMaxLength()
    {
        // Arrange
        var request = new UpdateAccreditationTaskStatusDto
        {
            AccreditationId = Guid.NewGuid(),
            TaskName = "CheckAccreditationStatus",
            Status = RegistrationTaskStatus.Completed,
            Comments = new string('x', 501)
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == ValidationMessages.AccreditationCommentsMaxLength);
    }

    [TestMethod]
    public void Validator_ShouldFail_WhenStatusIsQueriedAndCommentsAreEmpty()
    {
        // Arrange
        var request = new UpdateAccreditationTaskStatusDto
        {
            AccreditationId = Guid.NewGuid(),
            TaskName = "CheckAccreditationStatus",
            Status = RegistrationTaskStatus.Queried,
            Comments = "" // Should trigger the validation error
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == ValidationMessages.RegistrationCommentsRequired);
    }
}
