﻿using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentValidation;

namespace EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;
public class UpdateMaterialOutcomeRequestDtoValidator : AbstractValidator<UpdateMaterialOutcomeRequestDto>
{
    public UpdateMaterialOutcomeRequestDtoValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage(ValidationMessages.InvalidRegistrationOutcomeStatus);

        RuleFor(x => x.Comments)
            .MaximumLength(500)
            .WithMessage(ValidationMessages.RegistrationOutcomeCommentsMaxLength)
            .When(x => !string.IsNullOrEmpty(x.Comments));

        RuleFor(x => x.Comments)
            .NotEmpty()
            .WithMessage(ValidationMessages.RegistrationOutcomeCommentsCommentsRequired)
            .When(x => x.Status == ApplicationStatus.Refused);
    }
}
