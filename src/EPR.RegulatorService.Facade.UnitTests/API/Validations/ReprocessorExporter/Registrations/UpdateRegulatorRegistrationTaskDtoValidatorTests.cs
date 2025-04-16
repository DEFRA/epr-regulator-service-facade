using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPR.RegulatorService.Facade.UnitTests.API.Validations.ReprocessorExporter.Registrations
{
    [TestClass]
    public class UpdateRegulatorRegistrationTaskDtoValidatorTests
    {
        private readonly UpdateRegulatorRegistrationTaskDtoValidator _validator = new();

        [TestMethod]
        public void Validator_ShouldPass_WhenAllFieldsAreValid()
        {
            // Arrange
            var request = new UpdateRegulatorRegistrationTaskDto
            {
                RegistrationId = 1,
                TaskName = "Complete Task",
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
            var request = new UpdateRegulatorRegistrationTaskDto
            {
                RegistrationId = 1,
                TaskName = "Task",
                Status = (RegistrationTaskStatus)999,  // Invalid status
                Comments = "Test comment",
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
            var request = new UpdateRegulatorRegistrationTaskDto
            {
                RegistrationId = 2,
                TaskName = "Task",
                Status = RegistrationTaskStatus.Queried, // Status that requires comments
                Comments = "",  // Missing comments for 'Queried' status
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
            var request = new UpdateRegulatorRegistrationTaskDto
            {
                RegistrationId = 3,
                TaskName = "Task",
                Status = RegistrationTaskStatus.Completed,
                Comments = new string('x', 501), // 501 characters (too long)
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
            var request = new UpdateRegulatorRegistrationTaskDto
            {
                RegistrationId = 1,
                TaskName = "Test Task",
                Status = RegistrationTaskStatus.Queried,  // Status that requires comments
                Comments = "",  // Empty comments should fail
                UserName = "user"
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == ValidationMessages.RegistrationCommentsRequired);
        }
    }
}
