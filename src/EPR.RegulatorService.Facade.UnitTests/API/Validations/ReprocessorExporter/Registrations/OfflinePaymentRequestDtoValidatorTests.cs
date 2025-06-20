using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPR.RegulatorService.Facade.UnitTests.API.Validations.ReprocessorExporter.Registrations;

[TestClass]
public class OfflinePaymentRequestDtoValidatorTests
{
    private readonly OfflinePaymentRequestDtoValidator _validator = new();

    [TestMethod]
    public void Validator_ShouldPass_WhenAllRequiredFieldsAreProvided()
    {
        // Arrange
        var request = new OfflinePaymentRequestDto
        {
            PaymentReference = "REF12345",
            Regulator = "GB-ENG"
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
        var request = new OfflinePaymentRequestDto
        {
            PaymentReference = string.Empty,
            Regulator = "GB-ENG"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == ValidationMessages.OfflineReferenceRequired);
    }

    [TestMethod]
    public void Validator_ShouldFail_WhenRegulatorIsEmpty()
    {
        // Arrange
        var request = new OfflinePaymentRequestDto
        {
            PaymentReference = "REF12345",
            Regulator = string.Empty
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == ValidationMessages.OfflineRegulatorRequired);
    }

    [TestMethod]
    public void Validator_ShouldFail_WhenBothFieldsAreEmpty()
    {
        // Arrange
        var request = new OfflinePaymentRequestDto
        {
            PaymentReference = string.Empty,
            Regulator = string.Empty
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == ValidationMessages.OfflineReferenceRequired);
        result.Errors.Should().Contain(x => x.ErrorMessage == ValidationMessages.OfflineRegulatorRequired);
    }
}
