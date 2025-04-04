﻿using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentValidation;

namespace EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;
public class UpdateMaterialOutcomeRequestDtoValidator : AbstractValidator<UpdateMaterialOutcomeRequestDto>
{
    public UpdateMaterialOutcomeRequestDtoValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage(ValidationMessages.StatusRequired);

        RuleFor(x => x.Comments)
            .MaximumLength(500)
            .WithMessage(ValidationMessages.CommentsMaxLength);
    }
}
