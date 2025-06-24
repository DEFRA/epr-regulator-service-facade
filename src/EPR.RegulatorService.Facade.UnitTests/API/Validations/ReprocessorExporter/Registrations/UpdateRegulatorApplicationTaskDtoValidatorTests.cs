using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPR.RegulatorService.Facade.UnitTests.API.Validations.ReprocessorExporter.Registrations;

[TestClass]
public class UpdateRegulatorApplicationTaskDtoValidatorTests
{
    private readonly UpdateRegulatorApplicationTaskDtoValidator _validator = new();

    [TestMethod]
    public void Validator_ShouldPass_WhenAllFieldsAreValid()
    {
        // Arrange
        var request = new UpdateRegulatorApplicationTaskDto
        {
            RegistrationMaterialId = Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"),
            TaskName = "Review",
            Status = RegistrationTaskStatus.Completed,
            Comments = "All good",
            UserName = "test.user"
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
        var request = new UpdateRegulatorApplicationTaskDto
        {
            RegistrationMaterialId = Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"),
            TaskName = "Task",
            Status = (RegistrationTaskStatus)999,
            Comments = "",
            UserName = "user"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == ValidationMessages.InvalidRegistrationStatus);
    }

    [TestMethod]
    public void Validator_ShouldFail_WhenCommentsRequiredForQueriedStatus_ButMissing()
    {
        // Arrange
        var request = new UpdateRegulatorApplicationTaskDto
        {
            RegistrationMaterialId = Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"),
            TaskName = "Task",
            Status = RegistrationTaskStatus.Queried,
            Comments = "", // Required when status is Queried
            UserName = "user"
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
        var request = new UpdateRegulatorApplicationTaskDto
        {
            RegistrationMaterialId = Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"),
            TaskName = "Task",
            Status = RegistrationTaskStatus.Completed,
            Comments = new string('x', 501),
            UserName = "user"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == ValidationMessages.RegistrationCommentsMaxLength);
    }

    [TestMethod]
    public void Validator_ShouldFail_WhenStatusIsQueriedAndCommentsAreEmpty()
    {
        // Arrange
        var request = new UpdateRegulatorApplicationTaskDto
        {
            RegistrationMaterialId = Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"),
            TaskName = "Test",
            Status = RegistrationTaskStatus.Queried,
            Comments = "", // Should trigger the validation error
            UserName = "user"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == ValidationMessages.RegistrationCommentsRequired);
    }
}
