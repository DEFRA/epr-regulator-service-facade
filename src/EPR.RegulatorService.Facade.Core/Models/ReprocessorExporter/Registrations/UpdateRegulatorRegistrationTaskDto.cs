using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class UpdateRegulatorRegistrationTaskDto: UpdateRegulatorTaskBaseDto
{
    [Required]
    public required Guid RegistrationId { get; set; }
}
