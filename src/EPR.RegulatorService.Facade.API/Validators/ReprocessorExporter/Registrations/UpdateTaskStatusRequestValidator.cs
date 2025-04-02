using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Validators.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentValidation;

namespace EPR.RegulatorService.Facade.API.Validators.ReprocessorExporter.Registrations;

public class UpdateTaskStatusRequestValidator : AbstractValidator<UpdateTaskStatusRequestDto>
{
    public UpdateTaskStatusRequestValidator()
    {

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage(ValidationMessages.StatusRequired);

        When(x => x.Status == Core.Enums.RegistrationTaskStatus.Queried, () =>
        {
            RuleFor(x => x.Comments)
                .NotEmpty()
                .WithMessage(ValidationMessages.CommentsRequiredIfStatusIsQueried);
        });

        RuleFor(x => x.Comments)
            .MaximumLength(200)
            .WithMessage(ValidationMessages.CommentsMaxLength);
    }
}
