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
        public decimal TotalInputs { get; set; }
        public decimal TotalOutputs { get; set; }
        public string TaskStatus { get; set; }
        public Guid RegulatorApplicationTaskStatusId { get; set; }
        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; } = string.Empty;
        public string SiteAddress { get; set; }
    }
}