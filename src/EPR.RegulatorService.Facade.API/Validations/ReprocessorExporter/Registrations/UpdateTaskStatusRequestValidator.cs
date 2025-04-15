using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.API;
using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentValidation;

namespace EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;

public class UpdateTaskStatusRequestValidator : AbstractValidator<UpdateTaskStatusRequestDto>
{
    public UpdateTaskStatusRequestValidator()
    {
        // Status: Required
        RuleFor(x => x.Status)
            .NotNull()
            .WithMessage(ValidationMessages.InvalidRegistrationStatus)
            .Must(status => Enum.IsDefined(typeof(RegistrationTaskStatus), status))
            .WithMessage(ValidationMessages.InvalidRegistrationStatus);

        // Comments: Max Length 
        RuleFor(x => x.Comments)
            .MaximumLength(MaxLengths.UpdateTaskStatusRequestComments)
            .WithMessage(ValidationMessages.RegistrationCommentsMaxLength);

        // Comments: Optional, Required when Status = 'Queried'
        When(x => x.Status == RegistrationTaskStatus.Queried, () =>
        {
            RuleFor(x => x.Comments)
                .NotEmpty()
                .WithMessage(ValidationMessages.RegistrationCommentsRequired);
        });
    }
}
