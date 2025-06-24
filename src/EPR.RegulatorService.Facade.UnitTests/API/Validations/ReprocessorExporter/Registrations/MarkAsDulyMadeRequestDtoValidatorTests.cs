using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EPR.RegulatorService.Facade.UnitTests.API.Validations.ReprocessorExporter.Registrations;

[TestClass]
public class MarkAsDulyMadeRequestDtoValidatorTests
{
    private readonly MarkAsDulyMadeRequestDtoValidator _validator = new();

    [TestMethod]
    public void Validator_ShouldPass_WhenBothDatesAreProvided()
    {
        // Arrange
        var request = new MarkAsDulyMadeRequestDto
        {
            DulyMadeDate = DateTime.UtcNow,
            DeterminationDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public void Validator_ShouldFail_WhenDulyMadeDateIsMissing()
    {
        // Arrange
        var request = new MarkAsDulyMadeRequestDto
        {
            DulyMadeDate = default,
            DeterminationDate = DateTime.UtcNow
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == ValidationMessages.InvalidDulyMadeDate);
    }

    [TestMethod]
    public void Validator_ShouldFail_WhenDeterminationDateIsMissing()
    {
        // Arrange
        var request = new MarkAsDulyMadeRequestDto
        {
            DulyMadeDate = DateTime.UtcNow,
            DeterminationDate = default
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == ValidationMessages.InvalidDeterminationDate);
    }

    [TestMethod]
    public void Validator_ShouldFail_WhenBothDatesAreMissing()
    {
        // Arrange
        var request = new MarkAsDulyMadeRequestDto
        {
            DulyMadeDate = default,
            DeterminationDate = default
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == ValidationMessages.InvalidDulyMadeDate);
        result.Errors.Should().Contain(x => x.ErrorMessage == ValidationMessages.InvalidDeterminationDate);
    }
}