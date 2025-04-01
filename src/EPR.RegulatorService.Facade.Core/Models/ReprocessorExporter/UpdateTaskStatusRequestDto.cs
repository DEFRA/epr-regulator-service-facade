using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter;

public class UpdateTaskStatusRequestDto
{
    public RegistrationTaskStatus Status { get; set; }

    public string Comments { get; set; } = null;
}
