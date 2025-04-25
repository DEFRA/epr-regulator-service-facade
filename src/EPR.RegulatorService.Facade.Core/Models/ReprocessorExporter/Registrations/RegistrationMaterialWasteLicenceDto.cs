namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations
{
    public class RegistrationMaterialWasteLicenceDto
    {
        public required string PermitType { get; set; }
        public string[] LicenceNumbers { get; set; }

        public decimal? CapacityTonne { get; set; }
        public string? CapacityPeriod { get; set; }

        public decimal MaximumReprocessingCapacityTonne { get; set; }
        public string MaximumReprocessingPeriod { get; set; }
        public string MaterialName { get; set; }
    }
}