using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using System;
using System.Collections.Generic;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
public class RegistrationMaterialDto
{
    public int Id { get; init; }

    public int RegistrationId { get; init; }

    public required string MaterialName { get; init; }

    public DateTime? DeterminationDate { get; set; }

    public RegistrationMaterialStatus? Status { get; set; }

    public string? StatusUpdatedBy { get; init; }

    public DateTime? StatusUpdatedDate { get; init; }

    public string? RegistrationReferenceNumber { get; init; }

    public List<RegistrationTaskDto> Tasks { get; set; } = [];
}
