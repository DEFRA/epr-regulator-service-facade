using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPR.RegulatorService.Facade.UnitTests.API.Validations.ReprocessorExporter.Registrations;

[TestClass]
public class UpdateRegulatorRegistrationTaskDtoValidatorTests
{
    private readonly UpdateRegulatorRegistrationTaskDtoValidator _validator = new();

    [TestMethod]
    public void Should_ValidateSuccessfully_WhenInputIsCorrect()
    {
        var dto = new UpdateRegulatorRegistrationTaskDto
        {
            RegistrationId = Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"),
            TaskName = "Initial Review",
            Status = RegistrationTaskStatus.Completed,
            Comments = "Approved after verification.",
            UserName = "valid.user"
        };

        var validation = _validator.Validate(dto);

        validation.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public void Should_FailValidation_WhenProvidedStatusIsUnknown()
    {
        var input = new UpdateRegulatorRegistrationTaskDto
        {
            RegistrationId = Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"),
            TaskName = "Validation Task",
            Status = (RegistrationTaskStatus)12345,
            Comments = "Status invalid test",
            UserName = "auditor"
        };

        var validation = _validator.Validate(input);

        validation.IsValid.Should().BeFalse();
        validation.Errors.Should().Contain(x => x.ErrorMessage == ValidationMessages.InvalidRegistrationStatus);
    }

    [TestMethod]
    public void Should_EnforceCommentRequirement_ForQueriedStatus()
    {
        var task = new UpdateRegulatorRegistrationTaskDto
        {
            RegistrationId = Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"),
            TaskName = "Review Request",
            Status = RegistrationTaskStatus.Queried,
            Comments = null, 
            UserName = "reviewer"
        };

        var output = _validator.Validate(task);

        output.IsValid.Should().BeFalse();
        output.Errors.Should().Contain(err => err.ErrorMessage == ValidationMessages.RegistrationCommentsRequired);
    }

    [TestMethod]
    public void Should_ReturnError_WhenCommentsTooLong()
    {
        var dto = new UpdateRegulatorRegistrationTaskDto
        {
            RegistrationId = Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"),
            TaskName = "Long Comment Check",
            Status = RegistrationTaskStatus.Completed,
            Comments = new string('A', 600),
            UserName = "overwriter"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == ValidationMessages.RegistrationCommentsMaxLength);
    }

    [TestMethod]
    public void Should_FlagMissingComments_ForQueriedStatus()
    {
        var task = new UpdateRegulatorRegistrationTaskDto
        {
            RegistrationId = Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"),
            TaskName = "Escalation Review",
            Status = RegistrationTaskStatus.Queried,
            Comments = string.Empty, 
            UserName = "analyst"
        };

        var outcome = _validator.Validate(task);

        outcome.IsValid.Should().BeFalse();
        outcome.Errors.Should().Contain(fail => fail.ErrorMessage == ValidationMessages.RegistrationCommentsRequired);
    }
}
