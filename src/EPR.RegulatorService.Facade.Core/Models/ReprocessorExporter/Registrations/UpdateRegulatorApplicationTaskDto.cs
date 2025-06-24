using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class UpdateRegulatorApplicationTaskDto : UpdateRegulatorTaskBaseDto
{
    [Required]
    public Guid RegistrationMaterialId { get; set; }
}
