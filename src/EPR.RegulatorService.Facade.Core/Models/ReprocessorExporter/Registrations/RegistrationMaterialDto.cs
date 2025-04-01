using System;
using System.Collections.Generic;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
public class RegistrationMaterialDto
{
    public int Id { get; set; } 
    public string MaterialName { get; set; }
    public string Status { get; set; }
    public string ReferenceNumber { get; set; }
    public DateTime? DeterminationDate { get; set; }
    public List<RegistrationTaskDto> Tasks { get; set; }
}
