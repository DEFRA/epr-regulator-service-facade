using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
public class RegistrationOverviewDto
{
    public int Id { get; init; }

    public required string OrganisationName { get; init; } = string.Empty;

    public string? SiteAddress { get; init; }

    public ApplicationOrganisationType OrganisationType { get; init; }

    public required string Regulator { get; init; }

    public List<RegistrationTaskDto> Tasks { get; set; } = [];

    public List<RegistrationMaterialDto> Materials { get; set; } = [];
}
