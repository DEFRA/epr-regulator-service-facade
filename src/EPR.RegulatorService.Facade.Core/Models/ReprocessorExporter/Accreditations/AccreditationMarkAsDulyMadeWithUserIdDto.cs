using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;

public class AccreditationMarkAsDulyMadeWithUserIdDto : AccreditationMarkAsDulyMadeRequestDto
{
    public Guid DulyMadeBy { get; set; }
}
