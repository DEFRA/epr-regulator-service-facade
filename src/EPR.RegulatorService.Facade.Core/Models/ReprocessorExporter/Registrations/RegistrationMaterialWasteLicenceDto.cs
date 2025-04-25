namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations
{
    public class RegistrationMaterialWasteLicenceDto
    {
        public required string PermitType { get; set; }
        public string[] Number { get; set; }

        public decimal? CapacityTonne { get; set; }
        public string? Period { get; set; }

        public decimal MaximumReprocessingCapacityTonne { get; set; }
        public string MaximumReprocessingPeriod { get; set; }
        public string Material { get; set; }
    }
}