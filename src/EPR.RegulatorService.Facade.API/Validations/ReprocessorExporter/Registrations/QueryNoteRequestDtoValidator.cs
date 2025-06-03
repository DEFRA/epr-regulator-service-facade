using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using FluentValidation;

namespace EPR.RegulatorService.Facade.API.Validations.ReprocessorExporter.Registrations;

public class QueryNoteRequestDtoValidator : AbstractValidator<QueryNoteRequestDto>
{
    public QueryNoteRequestDtoValidator()
    {
        RuleFor(x => x.Note)
            .NotEmpty()
            .WithMessage(ValidationMessages.QueryNotesRequired);
    }
}
