using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;
using FluentValidation;

namespace EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Accreditations;

public class UpdateAccreditationMaterialTaskStatusDtoValidator : AbstractValidator<UpdateAccreditationMaterialTaskStatusDto>
{
    public UpdateAccreditationMaterialTaskStatusDtoValidator()
    {
        var allowedStatuses = new[] { AccreditationTaskStatus.Queried, AccreditationTaskStatus.Completed };

        RuleFor(x => x.TaskStatus)
                .Must(status => allowedStatuses.Contains(status))
                .WithMessage(ValidationMessages.InvalidAccreditationStatus);

        RuleFor(x => x.Comments)
                .MaximumLength(500).WithMessage(ValidationMessages.AccreditationCommentsMaxLength);

        RuleFor(x => x.Comments)
                .NotEmpty().When(x => x.TaskStatus == AccreditationTaskStatus.Queried)
                .WithMessage(ValidationMessages.AccreditationCommentsRequired);
    }
}
