using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class RegistrationTaskDto
{
    public Guid? Id { get; init; }
    public string TaskName { get; init; }
    public RegistrationTaskStatus Status { get; init; }
}
