using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class UpdateAccreditationTaskStatusWithUserIdDto : UpdateAccreditationTaskStatusDto
{
    [Required]
    public required Guid UpdatedByUserId { get; set; }
}
