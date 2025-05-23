﻿using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentAssertions;

namespace EPR.RegulatorService.Facade.UnitTests.API.Validations.ReprocessorExporter.Registrations;

[TestClass]
public class UpdateMaterialOutcomeRequestDtoValidatorTests
{
    private readonly UpdateMaterialOutcomeRequestDtoValidator _validator = new UpdateMaterialOutcomeRequestDtoValidator();

    [TestMethod]
    public void Validator_ShouldPass_WhenStatusIsValid()
    {
        // Arrange
        var request = new UpdateMaterialOutcomeRequestDto { Status = RegistrationMaterialStatus.Granted };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public void Validator_ShouldFail_WhenStatusIsInvalid()
    {
        // Arrange
        var request = new UpdateMaterialOutcomeRequestDto { Status = (RegistrationMaterialStatus)999 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == ValidationMessages.InvalidRegistrationStatus);
    }

    [TestMethod]
    public void Validator_ShouldPass_WhenCommentsAreValidAndStatusIsNotQueried()
    {
        // Arrange
        var request = new UpdateMaterialOutcomeRequestDto
        {
            Status = RegistrationMaterialStatus.Granted,
            Comments = new string('x', 500)
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public void Validator_ShouldFail_WhenCommentsExceedMaxLength()
    {
        // Arrange
        var request = new UpdateMaterialOutcomeRequestDto { Comments = new string('x', 501) };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == ValidationMessages.RegistrationCommentsMaxLength);
    }

    [TestMethod]
    public void Validator_ShouldFail_WhenCommentsAreRequiredButNotProvided_AndStatusIsRefused()
    {
        // Arrange
        var request = new UpdateMaterialOutcomeRequestDto { Status = RegistrationMaterialStatus.Refused };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == ValidationMessages.RegistrationCommentsRequired);
    }

    [TestMethod]
    public void Validator_ShouldPass_WhenCommentsAreProvidedAndStatusIsQueried()
    {
        // Arrange
        var request = new UpdateMaterialOutcomeRequestDto
        {
            Status = RegistrationMaterialStatus.Refused,
            Comments = new string('x', 500)
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
