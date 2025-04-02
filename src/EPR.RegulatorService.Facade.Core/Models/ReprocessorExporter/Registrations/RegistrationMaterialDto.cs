using EPR.RegulatorService.Facade.Core.Enums;
using System;
using System.Collections.Generic;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
public class RegistrationMaterialDto
{
    public int Id { get; init; }

    public int RegistrationId { get; init; }

    public required string MaterialName { get; init; }

    public DateTime? DeterminationDate { get; set; }

    public ApplicationStatus? Status { get; set; }

    public string? StatusUpdatedByName { get; init; }

    public DateTime? StatusUpdatedAt { get; init; }

    public string? RegistrationReferenceNumber { get; init; }

    public List<RegistrationTaskDto> Tasks { get; set; } = [];
}
