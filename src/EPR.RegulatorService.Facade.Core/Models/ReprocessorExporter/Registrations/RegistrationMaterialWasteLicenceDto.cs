namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations
{
    public class RegistrationMaterialWasteLicencesDto
    {
        public required string PermitType { get; set; }
        public string[] LicenceNumbers { get; set; }

        public decimal? CapacityTonne { get; set; }
        public string? CapacityPeriod { get; set; }

        public decimal MaximumReprocessingCapacityTonne { get; set; }
        public string MaximumReprocessingPeriod { get; set; }
        public string MaterialName { get; set; }
        public Guid RegistrationMaterialId { get; set; }
        public string TaskStatus { get; set; }
        public Guid RegulatorApplicationTaskStatusId { get; set; }
        public Guid RegistrationId { get; set; }
        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; } = string.Empty;
        public string SiteAddress { get; set; }
        public List<QueryNoteResponseDto> QueryNotes { get; set; }

    }
}