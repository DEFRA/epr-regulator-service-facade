using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;

public class UpdateAccreditationMaterialTaskStatusDto
{
    [Required]
    public Guid AccreditationId { get; set; }
    [Required]
    public Guid RegistrationMaterialId { get; set; }
    [Required]
    public required int TaskId { get; set; }
    [Required]
    public required AccreditationTaskStatus TaskStatus { get; set; }
    [MaxLength(500)]
    public string? Comments { get; set; } = string.Empty;
}

