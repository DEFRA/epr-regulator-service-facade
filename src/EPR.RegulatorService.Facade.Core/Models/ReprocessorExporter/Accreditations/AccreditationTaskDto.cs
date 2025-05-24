namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;

public class AccreditationTaskDto
{
    public int? Id { get; set; }
    public required string TaskName { get; set; }
    public required string Status { get; set; }
}
