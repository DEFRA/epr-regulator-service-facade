using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class RegistrationTaskDto
{
    public Guid? Id { get; init; }
    public required string TaskName { get; set; }
    public required string Status { get; set; }
    public int? Year { get; set; }
}
