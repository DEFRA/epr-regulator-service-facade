using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentValidation;

namespace EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;

public class UpdateMaterialOutcomeRequestDtoValidator : AbstractValidator<UpdateMaterialOutcomeRequestDto>
{
    public UpdateMaterialOutcomeRequestDtoValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage(ValidationMessages.InvalidRegistrationStatus);

        RuleFor(x => x.Comments)
            .MaximumLength(500)
            .WithMessage(ValidationMessages.RegistrationCommentsMaxLength);

        RuleFor(x => x.Comments)
            .NotEmpty()
            .WithMessage(ValidationMessages.RegistrationCommentsRequired)
            .When(x => x.Status == RegistrationMaterialStatus.Refused);
    }
}
