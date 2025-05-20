using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class UpdateMaterialOutcomeWithReferenceDto
{
    public RegistrationMaterialStatus Status { get; set; }
    public string? Comments { get; init; }
    public required string RegistrationReferenceNumber { get; set; }
}
