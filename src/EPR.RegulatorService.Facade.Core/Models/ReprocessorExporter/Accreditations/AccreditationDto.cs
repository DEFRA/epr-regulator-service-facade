using System.Collections.Generic;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;

public class AccreditationDto
{
    public int Id { get; set; }
    public int RegistrationMaterialId { get; set; }
    public string? Status { get; set; }
    public int AccreditationYear { get; init; }
    public List<AccreditationTaskDto> Tasks { get; set; } = [];

}
