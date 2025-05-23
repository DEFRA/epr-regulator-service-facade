using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class RegistrationOverviewDto
{
    public Guid Id { get; init; }
    public Guid OrganisationId { get; set; }
    public string OrganisationName { get; set; }
    public string? SiteAddress { get; init; }
    public ApplicationOrganisationType OrganisationType { get; init; }
    public required string Regulator { get; init; }
    public List<RegistrationTaskDto> Tasks { get; set; } = [];
    public List<RegistrationMaterialDto> Materials { get; set; } = [];
}
