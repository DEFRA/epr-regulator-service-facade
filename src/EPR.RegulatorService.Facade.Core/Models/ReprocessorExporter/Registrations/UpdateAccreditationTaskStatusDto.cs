using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class UpdateAccreditationTaskStatusDto : UpdateRegulatorTaskBaseDto
{
    [Required]
    public Guid AccreditationId { get; set; }
}