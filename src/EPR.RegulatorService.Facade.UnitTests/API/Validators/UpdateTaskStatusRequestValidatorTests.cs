using EPR.RegulatorService.Facade.API.Validators;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPR.RegulatorService.Facade.UnitTests.API.Validators;

[TestClass]
public class UpdateTaskStatusRequestValidatorTests
{
    private UpdateTaskStatusRequestValidator _validator;

    [TestInitialize]
    public void Setup()
    {
        _validator = new UpdateTaskStatusRequestValidator();
    }

    [TestMethod]
    public void ShouldHaveError_When_StatusIsEmpty()
    {
        // Arrange
        var model = new UpdateTaskStatusRequestDto { };
        
        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [TestMethod]
    public void ShouldHaveError_When_CommentsAreEmptyAndStatusIsQueried()
    {
        // Arrange
        var model = new UpdateTaskStatusRequestDto
        {
            Status = RegistrationTaskStatus.Queried,
            Comments = string.Empty
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Comments);
    }

    [TestMethod]
    public void ShouldHaveError_When_CommentsExceedMaxLength()
    {
        // Arrange
        var model = new UpdateTaskStatusRequestDto
        {
            Status = RegistrationTaskStatus.Complete, 
            Comments = new string('a', 201)
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Comments);
    }

    [TestMethod]
    public void ShouldNotHaveError_When_StatusIsValidAndCommentsNotRequired()
    {
        // Arrange
        var model = new UpdateTaskStatusRequestDto
        {
            Status = RegistrationTaskStatus.Complete,
            Comments = string.Empty
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
        result.ShouldNotHaveValidationErrorFor(x => x.Comments);
    }

    [TestMethod]
    public void ShouldNotHaveError_When_StatusIsQueriedAndCommentsAreValid()
    {
        // Arrange
        var model = new UpdateTaskStatusRequestDto
        {
            Status = RegistrationTaskStatus.Queried,
            Comments = "Reason for querying"
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}