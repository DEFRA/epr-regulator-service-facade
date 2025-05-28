using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;

public class UpdateAccreditationMaterialTaskStatusWithUserIdDto : UpdateAccreditationMaterialTaskStatusDto
{
    [Required]
    public required Guid UpdatedByUserId { get; set; }
}
