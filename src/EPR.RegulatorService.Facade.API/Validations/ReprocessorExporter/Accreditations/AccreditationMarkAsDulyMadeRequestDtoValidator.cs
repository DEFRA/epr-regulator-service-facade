using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentValidation;

namespace EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Accreditations;

public class AccreditationMarkAsDulyMadeRequestDtoValidator : AbstractValidator<AccreditationMarkAsDulyMadeRequestDto>
{
    public AccreditationMarkAsDulyMadeRequestDtoValidator()
    {
        RuleFor(x => x.DulyMadeDate)
        .Cascade(CascadeMode.Stop)
        .NotEmpty().WithMessage(ValidationMessages.InvalidDulyMadeDate);

        RuleFor(x => x.DeterminationDate)
       .Cascade(CascadeMode.Stop)
       .NotEmpty().WithMessage(ValidationMessages.InvalidDeterminationDate);
    }
}
