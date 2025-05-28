using System.Collections.Generic;
using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Models;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class AccreditationDto
{
    public Guid Id { get; set; }
    public int RegistrationMaterialId { get; set; }
    public string? Status { get; set; }
    public int AccreditationYear { get; set; }
    public string? ApplicationReference { get; set; }
    public DateTime? DeterminationDate { get; set; }
    public List<RegistrationTaskDto> Tasks { get; set; } = [];
}
