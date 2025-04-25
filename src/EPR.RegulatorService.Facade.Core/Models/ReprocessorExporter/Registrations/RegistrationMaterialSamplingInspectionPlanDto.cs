namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations
{
    public class RegistrationMaterialSamplingPlanDto
    {
        public string FileName { get; set; }
        public string FileDownloadUrl { get; set; }
        public DateTime DateUploaded { get; set; }
        public string UploadedBy { get; set; }
    }
}