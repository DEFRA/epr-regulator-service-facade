using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentValidation;

namespace EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;

public class MarkAsDulyMadeRequestDtoValidator : AbstractValidator<MarkAsDulyMadeRequestDto>
{
    public MarkAsDulyMadeRequestDtoValidator()
    {
        RuleFor(x => x.DulyMadeDate)
        .Cascade(CascadeMode.Stop)
        .NotEmpty().WithMessage(ValidationMessages.InvalidDulyMadeDate);

        RuleFor(x => x.DeterminationDate)
       .Cascade(CascadeMode.Stop)
       .NotEmpty().WithMessage(ValidationMessages.InvalidDeterminationDate);
    }
}
