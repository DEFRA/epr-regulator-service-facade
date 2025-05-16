using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentValidation;

namespace EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;

public class OfflinePaymentRequestDtoValidator : AbstractValidator<OfflinePaymentRequestDto>
{
    public OfflinePaymentRequestDtoValidator()
    {
        RuleFor(x => x.PaymentReference)
            .NotEmpty()
            .WithMessage(ValidationMessages.OfflineReferenceRequired);

        RuleFor(x => x.Regulator)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ValidationMessages.OfflineRegulatorRequired);
    }
}
