namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class RegistrationWasteCarrierDto
{
    public Guid RegistrationId { get; init; }
    public Guid OrganisationId { get; set; }
    public string OrganisationName { get; set; } = string.Empty;
    public string SiteAddress { get; set; } = string.Empty;
    public string? WasteCarrierBrokerDealerNumber { get; set; } = string.Empty;
    public Guid RegulatorRegistrationTaskStatusId { get; set; }
    public string TaskStatus { get; set; }
    public List<QueryNoteResponseDto> QueryNotes { get; set; }
}