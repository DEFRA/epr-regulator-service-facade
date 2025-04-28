namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations
{
    public class RegistrationMaterialSamplingPlanDto
    {
        public required string MaterialName { get; set; }
        public List<RegistrationMaterialSamplingPlanFileDto> Files { get; set; } = [];
    }
}