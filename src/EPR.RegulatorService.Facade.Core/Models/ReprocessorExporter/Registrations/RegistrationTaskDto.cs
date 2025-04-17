using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class RegistrationTaskDto
{
    public int? Id { get; init; }
    public RegistrationTaskType TaskName { get; init; }
    public RegistrationTaskStatus Status { get; init; }
}
