using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class UpdateTaskStatusRequestDto
{
    public RegistrationTaskStatus? Status { get; set; }

    public string? Comments { get; set; }
}