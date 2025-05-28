using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class AccreditationMarkAsDulyMadeRequestDto
{
    [Required]
    public Guid AccreditationId { get; set; }

    [Required]
    public Guid RegistrationMaterialId { get; set; }

    public DateTime DulyMadeDate { get; set; }
    public DateTime DeterminationDate { get; set; }
}
