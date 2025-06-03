namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations
{
    public class RegistrationMaterialSamplingPlanDto
    {
        public required string MaterialName { get; set; }
        public List<RegistrationMaterialSamplingPlanFileDto> Files { get; set; } = [];
        public string TaskStatus { get; set; }
        public Guid RegulatorApplicationTaskStatusId { get; set; }
        public Guid RegistrationId { get; set; }
        public Guid RegistrationMaterialId { get; set; }
        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; } = string.Empty;
        public string SiteAddress { get; set; }
        public List<QueryNoteResponseDto> QueryNotes { get; set; }
    }
}