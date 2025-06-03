using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPR.RegulatorService.Facade.UnitTests.API.Validations.ReprocessorExporter.Registrations;

[TestClass]
public class QueryNoteRequestDtoValidatorTests
{
    private readonly QueryNoteRequestDtoValidator _validator = new();

    [TestMethod]
    public void Validator_ShouldPass_WhenAllRequiredFieldsAreProvided()
    {
        // Arrange
        var request = new QueryNoteRequestDto
        {
            Note = "New Note",
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public void Validator_ShouldFail_WhenPaymentReferenceIsEmpty()
    {
        // Arrange
        var request = new QueryNoteRequestDto();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == ValidationMessages.QueryNotesRequired);
    }
}
