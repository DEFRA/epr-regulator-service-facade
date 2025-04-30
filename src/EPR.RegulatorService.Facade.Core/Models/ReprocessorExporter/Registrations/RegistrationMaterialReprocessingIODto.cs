namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations
{
    public class RegistrationMaterialReprocessingIODto
    {
        public required string MaterialName { get; set; }
        public required string SourcesOfPackagingWaste { get; set; }
        public required string PlantEquipmentUsed { get; set; }
        public bool ReprocessingPackagingWasteLastYearFlag { get; set; }
        public decimal UKPackagingWasteTonne { get; set; }
        public decimal NonUKPackagingWasteTonne { get; set; }
        public decimal NotPackingWasteTonne { get; set; }
        public decimal SenttoOtherSiteTonne { get; set; }
        public decimal ContaminantsTonne { get; set; }
        public decimal ProcessLossTonne { get; set; }
        public decimal TotalInput { get; set; }
        public decimal TotalOutput { get; set; }
    }
}