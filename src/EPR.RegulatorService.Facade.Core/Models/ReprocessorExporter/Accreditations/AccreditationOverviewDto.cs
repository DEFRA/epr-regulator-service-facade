using System;
using System.Collections.Generic;
using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;

public class AccreditationOverviewDto
{
    public int Id { get; set; }

    public required string OrganisationName { get; set; } = string.Empty;

    public string? SiteAddress { get; init; }

    public ApplicationOrganisationType OrganisationType { get; set; }

    public required string Regulator { get; set; }

    public List<AccreditationTaskDto> Tasks { get; set; } = [];

    public List<RegistrationMaterialDto> Materials { get; set; } = [];
}
