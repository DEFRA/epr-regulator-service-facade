namespace EPR.RegulatorService.Facade.API.Validators;

using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentValidation;

public class UpdateTaskStatusRequestValidator : AbstractValidator<UpdateTaskStatusRequestDto>
{
    public UpdateTaskStatusRequestValidator()
    {

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status is required.");

        When(x => x.Status == Core.Enums.RegistrationTaskStatus.Queried, () =>
        {
            RuleFor(x => x.Comments)
                .NotEmpty()
                .WithMessage("Comments field is required if 'Status' is queried.");
        });

        RuleFor(x => x.Comments)
            .MaximumLength(200)
            .WithMessage("Comments must not exceed 200 characters.");
    }
}
