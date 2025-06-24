using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class RegistrationMaterialDetailsDto
{
    public Guid Id { get; set; }   
    public Guid RegistrationId { get; set; }           
    public string MaterialName { get; set; }
    public RegistrationMaterialStatus? Status { get; set; }
}