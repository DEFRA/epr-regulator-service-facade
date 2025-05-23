using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using System;
using System.Collections.Generic;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class RegistrationMaterialDto
{
    public Guid Id { get; set; }
    public Guid RegistrationId { get; set; }
    public required string MaterialName { get; set; }
    public string? Status { get; set; }
    public string? StatusUpdatedBy { get; init; }
    public DateTime? StatusUpdatedDate { get; init; }
    public string? ApplicationReferenceNumber { get; init; }
    public string? RegistrationReferenceNumber { get; init; }
    public string? Comments { get; set; }
    public DateTime? DeterminationDate { get; set; }
    public bool IsMaterialRegistered { get; set; } = false;
    public List<RegistrationTaskDto> Tasks { get; set; } = [];
    public List<AccreditationDto> Accreditations { get; set; } = [];
}
