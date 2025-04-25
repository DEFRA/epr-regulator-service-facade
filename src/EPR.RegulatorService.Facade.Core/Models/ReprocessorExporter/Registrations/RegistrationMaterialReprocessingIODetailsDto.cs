namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations
{
    public class RegistrationMaterialReprocessingIODetailsDto
    {
        public List<string> PackagingWasteSources { get; set; }
        public List<string> PlantAndEquipment { get; set; }

        public string UkPackagingWasteInput { get; set; }
        public string NonUkPackagingWasteInput { get; set; }
        public string NonPackagingWasteInput { get; set; }
        public string TotalInput { get; set; }

        public string UkPackagingWasteOutput { get; set; }
        public string NonUkPackagingWasteOutput { get; set; }
        public string NonPackagingWasteOutput { get; set; }
        public string TotalOutput { get; set; }

        public string PlasticNotReprocessed { get; set; }
        public string ContaminantsFromReprocessing { get; set; }
        public string ProcessLoss { get; set; }
        public string TotalMaterialOutput { get; set; }
    }
}