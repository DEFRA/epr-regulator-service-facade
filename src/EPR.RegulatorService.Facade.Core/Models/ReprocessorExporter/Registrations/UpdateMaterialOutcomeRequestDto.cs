using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
public class UpdateMaterialOutcomeRequestDto
{
    public RegistrationTaskStatus Status { get; set; }
    public string? Comments { get; init; }
}

