using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Validators.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentValidation;

namespace EPR.RegulatorService.Facade.API.Validators.ReprocessorExporter.Registrations;

public class UpdateTaskStatusRequestValidator : AbstractValidator<UpdateTaskStatusRequestDto>
{
    public UpdateTaskStatusRequestValidator()
    {
        // Status: Required
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage(ValidationMessages.StatusRequired);

        // Comments: Max Length 
        RuleFor(x => x.Comments)
            .MaximumLength(200)
            .WithMessage(ValidationMessages.CommentsMaxLengthError);

        // Comments: Optional, Required when Status = 'Queried'
        When(x => x.Status == Core.Enums.RegistrationTaskStatus.Queried, () =>
        {
            RuleFor(x => x.Comments)
                .NotEmpty()
                .WithMessage(ValidationMessages.CommentsRequiredWhenStatusIsQueried);
        });
    }
}
