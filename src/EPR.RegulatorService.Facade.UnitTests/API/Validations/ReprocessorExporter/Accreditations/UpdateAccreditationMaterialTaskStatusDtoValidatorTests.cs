using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Accreditations;
using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;
using FluentAssertions;

namespace EPR.RegulatorService.Facade.UnitTests.API.Validations.ReprocessorExporter.Accreditations;

[TestClass]
public class UpdateAccreditationMaterialTaskStatusDtoValidatorTests
{
    private readonly UpdateAccreditationMaterialTaskStatusDtoValidator _validator = new();

    [TestMethod]
    public void Validator_ShouldPass_WhenAllFieldsAreValid()
    {
        // Arrange
        var request = new UpdateAccreditationMaterialTaskStatusDto()
        {
            AccreditationId = Guid.NewGuid(),
            TaskId = 10,
            RegistrationMaterialId = Guid.NewGuid(),
            TaskStatus = AccreditationTaskStatus.Queried,
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
        var request = new UpdateAccreditationMaterialTaskStatusDto
        {
            AccreditationId = Guid.NewGuid(),
            RegistrationMaterialId = Guid.NewGuid(),
            TaskId = 20,
            TaskStatus = (AccreditationTaskStatus)999,
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
        var request = new UpdateAccreditationMaterialTaskStatusDto
        {
            AccreditationId = Guid.NewGuid(),
            RegistrationMaterialId = Guid.NewGuid(),
            TaskId = 2,
            TaskStatus = AccreditationTaskStatus.Queried,
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
        var request = new UpdateAccreditationMaterialTaskStatusDto
        {
            AccreditationId = Guid.NewGuid(),
            RegistrationMaterialId = Guid.NewGuid(),
            TaskId = 5,
            TaskStatus = AccreditationTaskStatus.Completed,
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
        var request = new UpdateAccreditationMaterialTaskStatusDto
        {
            AccreditationId = Guid.NewGuid(),
            RegistrationMaterialId = Guid.NewGuid(),
            TaskId = 9,
            TaskStatus = AccreditationTaskStatus.Queried,
            Comments = "" // Should trigger the validation error
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == ValidationMessages.RegistrationCommentsRequired);
    }
}
