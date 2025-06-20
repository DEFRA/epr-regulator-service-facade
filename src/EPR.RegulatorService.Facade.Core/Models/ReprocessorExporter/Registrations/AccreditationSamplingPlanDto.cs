namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class AccreditationSamplingPlanDto
{
    public required string MaterialName { get; set; }
    public List<AccreditationSamplingPlanFileDto> Files { get; set; } = [];    
}
