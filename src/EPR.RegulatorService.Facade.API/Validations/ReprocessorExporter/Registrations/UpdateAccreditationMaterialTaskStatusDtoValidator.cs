using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentValidation;

namespace EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;

public class UpdateAccreditationMaterialTaskStatusDtoValidator : AbstractValidator<UpdateAccreditationTaskStatusDto>
{
    public UpdateAccreditationMaterialTaskStatusDtoValidator()
    {
        var allowedStatuses = new[] { RegistrationTaskStatus.Queried, RegistrationTaskStatus.Completed };

        RuleFor(x => x.Status)
                .Must(status => allowedStatuses.Contains(status))
                .WithMessage(ValidationMessages.InvalidAccreditationStatus);

        RuleFor(x => x.Comments)
                .MaximumLength(500).WithMessage(ValidationMessages.AccreditationCommentsMaxLength);

        RuleFor(x => x.Comments)
                .NotEmpty().When(x => x.Status == RegistrationTaskStatus.Queried)
                .WithMessage(ValidationMessages.AccreditationCommentsRequired);
    }
}